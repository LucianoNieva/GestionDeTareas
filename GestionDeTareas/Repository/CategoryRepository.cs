using GestionDeTareas.Datos;
using GestionDeTareas.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace GestionDeTareas.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Category>> ObtenerTodas()
        {
            return await dbSet
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Category?> ObtenerPorId(int id)
        {
            return await dbSet.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category?> ObtenerConTareas(int id)
        {
            return await dbSet
                .Include(c => c.TareasEnCategoria)
                .FirstOrDefaultAsync(c => c.Id == id); 
        }

        public async Task<List<Category>> ObtenerTodasConTareas()
        {
            return await dbSet
                .Include(c => c.TareasEnCategoria)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

        }

        public async Task<bool> Existe(string nombre)
        {
            return await dbSet.AnyAsync(c => c.Nombre == nombre);
        }

        public async Task<bool> ExisteConNombreExcluyendo(string nombre, int idExcluir)
        {
            return await dbSet
                .AnyAsync(c => c.Nombre == nombre && c.Id != idExcluir);
        }

    }
}
