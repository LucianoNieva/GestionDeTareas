using GestionDeTareas.Datos;
using GestionDeTareas.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace GestionDeTareas.Repository
{
    public class CategoryRepository : Repository<Categoria>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Categoria>> ObtenerPorUsuario(string userId)
        {
            return await dbSet
                .Where(c => c.IdUsuario == userId)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Categoria?> ObtenerPorIdYUsuario(int id, string userId)
        {
            return await dbSet.FirstOrDefaultAsync(c => c.Id == id && c.IdUsuario == userId);
        }

        public async Task<Categoria?> ObtenerConTareas(int id, string userId)
        {
            return await dbSet.Include(c => c.TareasEnCategoria).FirstOrDefaultAsync(c => c.Id == id && c.IdUsuario == userId); 
        }

        public async Task<List<Categoria>> ObtenerTodasConTareas(string userId)
        {
            return await dbSet.Where(c => c.IdUsuario == userId)
                .Include(c => c.TareasEnCategoria)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<bool> Exist(string nombre, string userId)
        {
            return await dbSet.AnyAsync(c => c.Nombre == nombre && c.IdUsuario == userId);
        }

        public async Task<bool> ExisteConNombre(string nombre, string userId)
        {
            return await dbSet
                .AnyAsync(c => c.Nombre == nombre && c.IdUsuario == userId);
        }

        public async Task<bool> ExisteConNombreExcluyendo(string nombre, string userId, int idExcluir)
        {
            return await dbSet
                .AnyAsync(c => c.Nombre == nombre
                            && c.IdUsuario == userId
                            && c.Id != idExcluir);
        }

    }
}
