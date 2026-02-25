using GestionDeTareas.Datos;
using GestionDeTareas.DTO.Tarea;
using GestionDeTareas.DTO.Usuario;
using GestionDeTareas.Models;
using GestionDeTareas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionDeTareas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TareaController : ControllerBase
    {
        private readonly TareaService _tareaService;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<TareaController> _logger;
        private readonly IUsuarioService _usuarioService;

        public TareaController(TareaService tareaService, ICurrentUserService currentUser, ILogger<TareaController> logger, IUsuarioService usuarioService)
        {
            _tareaService = tareaService;
            _currentUser = currentUser;
            _logger = logger;
            _usuarioService = usuarioService;
        }

        
        [HttpGet]
        public async Task<ActionResult<List<TareaDTO>>> GetMisTareas()
        {
            var esAdmin = _currentUser.IsInRole("Admin");
            var tareas = await _tareaService.ObtenerTareasSegunRol(_currentUser.UserId, esAdmin);
            return Ok(tareas);
        }

        [HttpGet("{id}", Name = "ObtenerTarea")]
        public async Task<ActionResult<TareaDetalleDTO>> GetTarea(int id)
        {
            
                var tarea = await _tareaService.ObtenerTareaPorId(id, _currentUser.UserId);
                if (tarea == null) 
                return NotFound(new { message = "Tarea no encontrada" });
            return Ok(tarea);
            
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CrearTarea(CreacionTareaDTO dto)
        {
            try
            {
                var (exito, error, tarea) = await _tareaService.CrearTarea(dto);

                if (!exito)
                    return BadRequest(new { message = error });

                _logger.LogInformation(
                    "Tarea creada: {TareaId} por usuario {UserId} ({Email})",
                    tarea!.Id,
                    _currentUser.UserId,
                    _currentUser.Email);

                return CreatedAtRoute("ObtenerTarea", new { id = tarea.Id }, tarea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tarea para usuario {UserId}", _currentUser.UserId);
                return StatusCode(500);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActualizarTarea(int id, ActualizarTareaDTO dto)
        {
            try
            {
                var (exito, error) = await _tareaService.ActualizarTarea(id, dto, _currentUser.UserId);

                if (!exito)
                    return NotFound(new { message = error });

                _logger.LogInformation("Tarea actualizada: {TareaId} por usuario {UserId}", id, _currentUser.UserId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tarea {TareaId}", id);
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> BorrarTarea(int id)
        {

            try
            {
                var (exito, error) = await _tareaService.EliminarTarea(id, _currentUser.UserId);

                if (!exito)
                    return NotFound(new { message = error });

                _logger.LogInformation("Tarea eliminada: {TareaId} por Admin {UserId}", id, _currentUser.UserId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tarea {TareaId}", id);
                return StatusCode(500, new { message = "Error al eliminar tarea" });
            }


        }

        [HttpPatch("{id}/completar")]
        public async Task<ActionResult> CompletarTarea(int id)
        {
            try
            {
                var (exito, error) = await _tareaService.CompletarTarea(id, _currentUser.UserId);

                if (!exito)
                    return NotFound(new { message = error });

                _logger.LogInformation("Tarea completada: {TareaId} por usuario {UserId}", id, _currentUser.UserId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar tarea {TareaId}", id);
                return StatusCode(500);
            }
        }

        [HttpGet("filtrar")]
        public async Task<ActionResult<List<TareaDTO>>> FiltrarTarea([FromQuery] Estado? estado,
            [FromQuery] Prioridad? prioridad,
            [FromQuery] int? categoriaId,
            [FromQuery] string? buscar)
        {
            try
            {
                var tareas = await _tareaService.FiltrarTareas(_currentUser.UserId, estado, prioridad, categoriaId, buscar);

                _logger.LogInformation(
                    "Filtrado de tareas - Usuario: {UserId}, Resultados: {Count}",
                    _currentUser.UserId,
                    tareas.Count);

                return Ok(tareas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar tareas");
                return StatusCode(500);
            }

        }
    }
}
