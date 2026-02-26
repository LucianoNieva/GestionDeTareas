using GestionDeTareas.Models;

namespace GestionDeTareas.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> ObtenerTodas();
        Task<Category?> ObtenerPorId(int id);
        Task<Category?> ObtenerConTareas(int id);
        Task<List<Category>> ObtenerTodasConTareas();
        Task<bool> Existe(string nombre);
        Task<bool> ExisteConNombreExcluyendo(string nombre, int idExcluir);
    }
}
