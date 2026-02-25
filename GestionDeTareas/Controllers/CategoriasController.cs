using GestionDeTareas.DTO.Category;
using GestionDeTareas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace GestionDeTareas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<CategoriasController> _logger;

         public CategoriasController(
            ICategoryService categoryService,
            ICurrentUserService currentUser,
            ILogger<CategoriasController> logger)
        {
            _categoryService = categoryService;
            _currentUser = currentUser;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategorias()
        {
           
            
                var categorias = await _categoryService.ObtenerTodasLasCategorias();
                return Ok(categorias);
            
           
        }

        [HttpGet("{id}", Name = "ObtenerCategoria")]
        public async Task<ActionResult<CategoryDTO>> GetCategoria(int id)
        {
            
                var categoria = await _categoryService.ObtenerCategoriaPorId(id);
            if (categoria == null)  // ✅ Validación agregada
                return NotFound(new { message = "Categoría no encontrada" });
            return Ok(categoria);
           
        }

        [HttpGet("{id}/tarea")]
        public async Task<ActionResult> GetTareasDeCategoria(int id)
        {

            var resultado = await _categoryService.ObtenerTareasDeCategoria(id);
            if (resultado == null)
                return NotFound(new { message = "Categoría no encontrada" });
            return Ok(resultado);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDTO>> CrearCategoria(CategoryCreacionDTO dto)
        {
            try
            {
                var (exito, error, categoria) = await _categoryService.CrearCategoriaGlobal(dto);

                if (!exito)
                    return BadRequest(new { message = error });

                _logger.LogInformation("Categoría creada: {CategoriaId}}", categoria!.Id);

                return CreatedAtRoute("ObtenerCategoria", new { id = categoria.Id }, categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActualizarCategoria(int id, CategoryCreacionDTO dto)
        {
            try
            {
                var (exito, error) = await _categoryService.ActualizarCategoria(id, dto);

                if (!exito)
                    return NotFound(new { message = error });

                _logger.LogInformation("Categoría actualizada: {CategoriaId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar categoría {Id}", id);
                return StatusCode(500);
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EliminarCategoria(int id)
        {
            try
            {
                var (exito, error) = await _categoryService.EliminarCategoria(id);

                if (!exito)
                    return BadRequest(new { message = error });

                _logger.LogInformation("Categoría eliminada: {CategoriaId} por usuario {UserId}", id, _currentUser.UserId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar categoría {Id}", id);
                return StatusCode(500);
            }
        }



    }
}
