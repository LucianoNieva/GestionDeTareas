using AutoMapper;
using GestionDeTareas.Datos;
using GestionDeTareas.DTO.Tarea;
using GestionDeTareas.Models;
using GestionDeTareas.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionDeTareas.Services
{
    public class TareaService: ITareaService
    {
        private readonly ITareaRepository _tareaRepository;
        private readonly ICategoryRepository _categoriaRepository;
        private readonly IUsuarioService _userManager;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public TareaService(
        ITareaRepository tareaRepository,
        ICategoryRepository categoriaRepository,
        IUsuarioService userManager,
        IMapper mapper,
        ICurrentUserService currentUserService)
        {
            _tareaRepository = tareaRepository;
            _categoriaRepository = categoriaRepository;
            _userManager = userManager;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }


        public async Task<List<TareaDTO>> ObtenerTareasUsuario(string userId)
        {
            var tareas = await _tareaRepository.ObtenerPorUsuario(userId);
            return _mapper.Map<List<TareaDTO>>(tareas);
        }

        public async Task<TareaDetalleDTO?> ObtenerTareaPorId(int id, string userId)
        {

            var tarea = await _tareaRepository.ObtenerConCategoria(id, userId);

            if (tarea == null) return null;

            return _mapper.Map<TareaDetalleDTO>(tarea);
        }

        public async Task<List<TareaDTO>?> ObtenerTareasSegunRol(string userId, bool esAdmin)
        {
            var tareas = await _tareaRepository.ObtenerTareasSegunRol(userId, esAdmin);
            if (tareas == null) return null;
            return _mapper.Map<List<TareaDTO>>(tareas);
        }   

        public async Task<(bool exito, string? error, TareaDTO? tarea)> CrearTarea(CreacionTareaDTO creacionTarea)
        {
            if(creacionTarea.IdCategoria.HasValue)
            {
                var categoriaExiste = await _categoriaRepository.ObtenerPorId(creacionTarea.IdCategoria.Value);

                if (categoriaExiste == null) return (false, "La categoria no existe", null);
            }

            if (!string.IsNullOrEmpty(creacionTarea.IdUsuario))
            {
                if (!await _userManager.ExisteUsuarioAsync(creacionTarea.IdUsuario))
                {
                    return (false, "El usuario asignado no existe", null);
                }
            }

            var tarea = _mapper.Map<Tarea>(creacionTarea);
            tarea.IdAdmin = _currentUserService.UserId;

            await _tareaRepository.Add(tarea);
            await _tareaRepository.Save();

            var tareaDTO = _mapper.Map<TareaDTO>(tarea);
            return (true, null , tareaDTO);
        }

        public async Task<(bool exito, string? error)> ActualizarTarea(int id, ActualizarTareaDTO actualizarTareaDTO, string userId)
        {
            var tarea = await _tareaRepository.ObtenerPorIdYUsuario(id, userId);

            if (tarea == null) return (false, "Tarea no encontrada");

            if (actualizarTareaDTO.IdCategoria.HasValue)
            {
                var categoriaExiste = await _categoriaRepository.ObtenerPorId(actualizarTareaDTO.IdCategoria.Value);

                if (categoriaExiste == null)
                {
                    return (false, "La categoría especificada no existe");
                }
            }

            _mapper.Map(actualizarTareaDTO, tarea);
            await _tareaRepository.Update(tarea);
            await _tareaRepository.Save();

            return (true, null);

        }

        public async Task<(bool exito, string? error)> EliminarTarea(int id, string userId)
        {
            var tarea = await _tareaRepository.ObtenerPorIdYUsuario(id, userId);

            if (tarea == null)
            {
                return (false, "Tarea no encontrada");
            }

            await _tareaRepository.Delete(tarea);
            await _tareaRepository.Save();

            return (true, null);
        }

        public async Task<(bool exito, string? error)> CompletarTarea(int id, string userId)
        {
            var tarea = await _tareaRepository.ObtenerPorIdYUsuario(id, userId);

            if (tarea == null)
            {
                return (false, "Tarea no encontrada");
            }

            tarea.Estado = Estado.Terminado;
            

            await _tareaRepository.Update(tarea);
            await _tareaRepository.Save();

            return (true, null);
        }

        public async Task<List<TareaDTO>> FiltrarTareas(
            string userId,
            Estado? estado,
            Prioridad? prioridad,
            int? categoriaId,
            string? buscar)
        {
            var tareas = await _tareaRepository.Filtrar(userId, estado, prioridad, categoriaId, buscar);
            return _mapper.Map<List<TareaDTO>>(tareas);
        }
    }
}
