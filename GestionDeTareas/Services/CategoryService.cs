using AutoMapper;
using GestionDeTareas.DTO.Category;
using GestionDeTareas.Models;
using GestionDeTareas.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace GestionDeTareas.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public async Task<List<CategoryDTO>> ObtenerCategoriasUsuario(string userId)
        {
            var categorias = await categoryRepository.ObtenerTodasConTareas(userId);
            return mapper.Map<List<CategoryDTO>>(categorias);
        }

        public async Task<CategoryDTO?> ObtenerCategoriaPorId(int id, string userId)
        {
            var categoria = await categoryRepository.ObtenerConTareas(id, userId);
            return mapper.Map<CategoryDTO>(categoria);
        }

        public async Task<(bool exito, string? error, CategoryDTO? categoria)> CrearCategoria(
            CategoryCreacionDTO dto, string userId)
        {
            // Validar nombre duplicado
            var existe = await categoryRepository.ExisteConNombre(dto.Nombre, userId);

            if (existe)
            {
                return (false, "Ya existe una categoría con ese nombre", null);
            }

            // Crear categoría
            var categoria = mapper.Map<Categoria>(dto);
            categoria.IdUsuario = userId;

            await categoryRepository.Add(categoria);
            await categoryRepository.Save();

            var categoriaDTO = mapper.Map<CategoryDTO>(categoria);

            return (true, null, categoriaDTO);
        }

        public async Task<(bool exito, string? error)> ActualizarCategoria(int id, CategoryCreacionDTO dto, string userId)
        {
            var categoria = await categoryRepository.ObtenerPorIdYUsuario(id, userId);

            if (categoria == null) return (false, "Categoria no encontrada");

            var existeNombre = await categoryRepository.ExisteConNombreExcluyendo(dto.Nombre, userId, id);

            if (existeNombre) return (false, "Ya existe otra categoria con ese nombre");
            mapper.Map(dto, categoria);
            await categoryRepository.Update(categoria);
            await categoryRepository.Save();

            return (true, null);
        }

        public async Task<(bool exito, string? error)> EliminarCategoria(int id, string userId)
        {
            var categoria = await categoryRepository.ObtenerConTareas(id, userId);

            if (categoria == null) return (false, "Categoria no encontrada");

            if (categoria.TareasEnCategoria!.Any()) return (false, $"No se puede eliminar porque tiene {categoria.TareasEnCategoria!.Count()} tarea/s asociada/s");

            await categoryRepository.Delete(categoria);
            await categoryRepository.Save();

            return (true, null);
        }

        public async Task<object?> ObtenerTareasDeCategoria(int id, string userId)
        {
            var categoria = await categoryRepository.ObtenerConTareas(id, userId);

            if (categoria == null)
                return null;

            return new
            {
                categoria = categoria.Nombre,
                cantidadTareas = categoria.TareasEnCategoria?.Count,
                tareas = categoria.TareasEnCategoria!.Select(t => new
                {
                    t.Id,
                    t.Titulo,
                    t.Estado,
                    t.Prioridad
                })
            };
        }
    }

}
