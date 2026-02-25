using GestionDeTareas.Models;

namespace GestionDeTareas.Repository
{
    public interface ICategoryRepository : IRepository<Categoria>
    {
        Task<List<Categoria>> ObtenerTodas();
        Task<Categoria?> ObtenerPorId(int id);
        Task<Categoria?> ObtenerConTareas(int id);
        Task<List<Categoria>> ObtenerTodasConTareas();
        Task<bool> Existe(string nombre);
        Task<bool> ExisteConNombreExcluyendo(string nombre, int idExcluir);
    }
}
