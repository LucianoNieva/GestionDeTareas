using GestionDeTareas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionDeTareas.Datos
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tarea> Tareas {  get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);  // ← IMPORTANTE: llamar al base

            // Configurar relación Usuario-Tareas
            builder.Entity<Tarea>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Tareas)
                .HasForeignKey(t => t.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar relación Categoria-Tareas
            builder.Entity<Tarea>()
                .HasOne(t => t.Categoria)
                .WithMany(c => c.TareasEnCategoria)
                .HasForeignKey(t => t.IdCategoria)
                .OnDelete(DeleteBehavior.NoAction);

            // Configurar relación Usuario-Categorias
            builder.Entity<Categoria>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Categorias)
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
