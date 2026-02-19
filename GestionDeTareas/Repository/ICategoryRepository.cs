using GestionDeTareas.Models;

namespace GestionDeTareas.Repository
{
    public interface ICategoryRepository : IRepository<Categoria>
    {
        Task<List<Categoria>> ObtenerPorUsuario(string userId);
        Task<Categoria?> ObtenerPorIdYUsuario(int id, string userId);
        Task<Categoria?> ObtenerConTareas(int id, string userId);
        Task<List<Categoria>> ObtenerTodasConTareas(string userId);
        Task<bool> ExisteConNombre(string nombre, string userId);
        Task<bool> ExisteConNombreExcluyendo(string nombre, string userId, int idExcluir);
    }
}
