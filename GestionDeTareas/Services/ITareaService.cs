using GestionDeTareas.DTO.Tarea;
using GestionDeTareas.Models;

namespace GestionDeTareas.Services
{
    public interface ITareaService
    {
        Task<List<TareaDTO>> ObtenerTareasUsuario(string userId);

        Task<TareaDetalleDTO?> ObtenerTareaPorId(int id, string userId);

        Task<List<TareaDTO>?> ObtenerTareasSegunRol(string userId, bool esAdmin);

        Task<(bool exito, string? error, TareaDTO? tarea)> CrearTarea(CreacionTareaDTO creacionTarea);

        Task<(bool exito, string? error)> ActualizarTarea(int id, ActualizarTareaDTO actualizarTareaDTO, string userId);

        Task<(bool exito, string? error)> EliminarTarea(int id, string userId);

        Task<(bool exito, string? error)> CompletarTarea(int id, string userId);

        Task<List<TareaDTO>> FiltrarTareas(
            string userId,
            Estado? estado,
            Prioridad? prioridad,
            int? categoriaId,
            string? buscar);
    }
}