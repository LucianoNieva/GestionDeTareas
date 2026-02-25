using GestionDeTareas.DTO.Usuario;
using GestionDeTareas.Models;
using GestionDeTareas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeTareas.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize(Roles = "Admin")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            IUsuarioService usuarioService,
            ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        // ========== LISTAR USUARIOS ==========
        [HttpGet]
        public async Task<ActionResult<List<UsuarioDTO>>> ListarUsuarios()
        {
            // El servicio ya hace el Join optimizado que armamos
            var usuariosDTO = await _usuarioService.GetUsuariosDisponiblesAsync();
            return Ok(usuariosDTO);
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearUsuario(UsuarioCreacionDTO dto)
        {
            // Llamamos al servicio pasando el rol que viene en el DTO
            var resultado = await _usuarioService.CrearUsuarioConRolAsync(dto, dto.Rol);

            if (!resultado.Succeeded)
            {
                return BadRequest(new
                {
                    mensaje = $"No se pudo crear el {dto.Rol}",
                    errores = resultado.Errors.Select(e => e.Description)
                });
            }

            _logger.LogInformation("{Rol} creado por Admin: {Email}", dto.Rol, dto.Email);

            return Ok(new { mensaje = $"{dto.Rol} creado con éxito" });
        }

        [HttpPut("cambiar-rol")]
        public async Task<IActionResult> CambiarRol(CambiarRolDTO dto)
        {
            // El servicio ya se encarga de buscar al usuario, remover roles viejos
            // y asignar el nuevo rol que viene validado desde el DTO.
            var resultado = await _usuarioService.CambiarRolAsync(dto.UserId, dto.NuevoRol);

            if (!resultado.Succeeded)
            {
                return BadRequest(new
                {
                    mensaje = "Error al intentar cambiar el rol",
                    errores = resultado.Errors.Select(e => e.Description)
                });
            }

            _logger.LogInformation("Admin cambió el rol del usuario {UserId} a {NuevoRol}", dto.UserId, dto.NuevoRol);

            return Ok(new { mensaje = $"El usuario ahora tiene el rol de {dto.NuevoRol}" });
        }
        // ========== ELIMINAR USUARIO ==========
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(string id)
        {
            var resultado = await _usuarioService.EliminarUsuarioAsync(id);

            if (!resultado.Succeeded)
            {
                return BadRequest(new { mensaje = "No se pudo eliminar el usuario", errores = resultado.Errors.Select(e => e.Description) });
            }

            _logger.LogInformation("Usuario con ID {Id} eliminado por Admin", id);
            return NoContent();
        }
    }
}