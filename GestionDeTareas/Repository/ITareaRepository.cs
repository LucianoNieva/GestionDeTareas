using GestionDeTareas.Models;

namespace GestionDeTareas.Repository
{
    public interface ITareaRepository : IRepository<Tarea>
    {
        Task<List<Tarea>> ObtenerPorUsuario(string userId);
        Task<Tarea?> ObtenerPorIdYUsuario(int id, string userId);
        Task<Tarea?> ObtenerConCategoria(int id, string userId);
        Task<List<Tarea>> ObtenerPorEstado(string userId, Estado estado);
        Task<List<Tarea>> ObtenerPorPrioridad(string userId, Prioridad prioridad);
        Task<List<Tarea>> ObtenerPorCategoria(string userId, int categoriaId);

        Task<List<Tarea>> ObtenerTareasSegunRol(string userId, bool esAdmin);
        Task<List<Tarea>> Filtrar(string userId, Estado? estado, Prioridad? prioridad, int? categoriaId, string? buscar);
        Task<int> ContarPorUsuario(string userId);
        Task<int> ContarPorEstado(string userId, Estado estado);
    }
}
