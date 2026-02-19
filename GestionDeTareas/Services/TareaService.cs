using AutoMapper;
using GestionDeTareas.Datos;
using GestionDeTareas.DTO.Tarea;
using GestionDeTareas.Models;
using GestionDeTareas.Repository;
using Microsoft.EntityFrameworkCore;

namespace GestionDeTareas.Services
{
    public class TareaService
    {
        private readonly ITareaRepository _tareaRepository;
        private readonly ICategoryRepository _categoriaRepository;
        private readonly IMapper _mapper;

        public TareaService(
            ITareaRepository tareaRepository,
            ICategoryRepository categoriaRepository,
            IMapper mapper)
        {
            _tareaRepository = tareaRepository;
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
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

        public async Task<(bool exito, string? error, TareaDTO? tarea)> CrearTarea(CreacionTareaDTO creacionTarea, string userId)
        {
            if(creacionTarea.IdCategoria.HasValue)
            {
                var categoriaExiste = await _categoriaRepository.ObtenerPorIdYUsuario(creacionTarea.IdCategoria.Value, userId);

                if (categoriaExiste == null) return (false, "La categoria no existe", null);
            }

            var tarea = _mapper.Map<Tarea>(creacionTarea);
            tarea.IdUsuario = userId;

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
                var categoriaExiste = await _categoriaRepository.ObtenerPorIdYUsuario(actualizarTareaDTO.IdCategoria.Value, userId);

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
