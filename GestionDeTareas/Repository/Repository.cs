using GestionDeTareas.Datos;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics;

namespace GestionDeTareas.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext context;
        protected readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public virtual async Task<List<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetById(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<T> Add(T item)
        {
            await dbSet.AddAsync(item);
            return item;
        }

        public virtual async Task Update(T item)
        {
            dbSet.Update(item);
            await Task.CompletedTask;
        }

        public virtual async Task Delete(T item)
        {
            dbSet.Remove(item);
            await Task.CompletedTask;
        }

        public virtual async Task<bool> Exist(int id)
        {
            var entidad = await dbSet.FindAsync(id);
            return entidad != null;
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
