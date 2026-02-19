using GestionDeTareas.Datos;
using GestionDeTareas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionDeTareas.Repository
{
    public class TareaRepository : Repository<Tarea>, ITareaRepository
    {
        public TareaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Tarea>> ObtenerPorUsuario(string userId)
        {
            return await dbSet
                .Where(c => c.IdUsuario == userId)
                .Include(c => c.Categoria)
                .OrderBy(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Tarea?> ObtenerPorIdYUsuario(int id, string userId)
        {
            return await dbSet.FirstOrDefaultAsync(c => c.Id == id && c.IdUsuario == userId);
        }

        public async Task<Tarea?> ObtenerConCategoria(int id, string userId)
        {
            return await dbSet
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.Id == id && t.IdUsuario == userId);
        }

        public async Task<List<Tarea>> ObtenerPorEstado(string UserId, Estado estado)
        {
            return await dbSet
                .Where(c => c.IdUsuario == UserId && c.Estado == estado)
                .Include (c => c.Categoria)
                .OrderBy(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<List<Tarea>> ObtenerPorPrioridad(string userId, Prioridad prioridad)
        {
            return await dbSet
                .Where(c => c.IdUsuario == userId &&  c.Prioridad == prioridad)
                .Include(c => c.Categoria)
                .OrderBy(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<List<Tarea>> ObtenerPorCategoria(string userId, int categoriaId)
        {
            return await dbSet
                .Where(c => c.IdUsuario == userId && c.IdCategoria == categoriaId)
                .Include(c => c.Categoria)
                .OrderBy(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<List<Tarea>> Filtrar(
            string userId,
            Estado? estado,
            Prioridad? prioridad,
            int? categoriaId,
            string? buscar)
        {
            var query = dbSet
                .Where(t => t.IdUsuario == userId)
                .Include(t => t.Categoria)
                .AsQueryable();

            if (estado.HasValue)
                query = query.Where(t => t.Estado == estado.Value);

            if (prioridad.HasValue)
                query = query.Where(t => t.Prioridad == prioridad.Value);

            if (categoriaId.HasValue)
                query = query.Where(t => t.IdCategoria == categoriaId.Value);

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                query = query.Where(t =>
                    t.Titulo.Contains(buscar));
            }

            return await query
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();
        }

        public async Task<int> ContarPorUsuario(string userId)
        {
            return await dbSet
                .CountAsync(t => t.IdUsuario == userId);
        }

        public async Task<int> ContarPorEstado(string userId, Estado estado)
        {
            return await dbSet
                .CountAsync(t => t.IdUsuario == userId && t.Estado == estado);
        }


    }
}
