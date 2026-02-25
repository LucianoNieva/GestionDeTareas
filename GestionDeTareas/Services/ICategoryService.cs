using GestionDeTareas.DTO.Category;

namespace GestionDeTareas.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> ObtenerTodasLasCategorias();
        Task<CategoryDTO?> ObtenerCategoriaPorId(int id);
        Task<(bool exito, string? error, CategoryDTO? categoria)> CrearCategoriaGlobal(CategoryCreacionDTO dto);
        Task<(bool exito, string? error)> ActualizarCategoria(int id, CategoryCreacionDTO dto);
        Task<(bool exito, string? error)> EliminarCategoria(int id);
        Task<object?> ObtenerTareasDeCategoria(int id);
    }
}