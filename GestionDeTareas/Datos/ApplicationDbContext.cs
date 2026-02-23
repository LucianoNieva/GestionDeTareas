using GestionDeTareas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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

            builder.Entity<Tarea>()
                 .HasOne(t => t.Admin)
                 .WithMany(u => u.Tareas)
                 .HasForeignKey(t => t.IdAdmin)
                 .OnDelete(DeleteBehavior.Restrict);

            // Relación: Tarea -> UsuarioAsignado (a quien se asignó)
           builder.Entity<Tarea>()
                .HasOne(t => t.Usuario)
                .WithMany()  // Sin colección inversa en Usuario
                .HasForeignKey(t => t.IdUsuario)
                .IsRequired(false)  // Puede ser NULL
                .OnDelete(DeleteBehavior.SetNull);

            // Relación: Tarea -> Categoria
           builder.Entity<Tarea>()
                .HasOne(t => t.Categoria)
                .WithMany(c => c.TareasEnCategoria)
                .HasForeignKey(t => t.IdCategoria)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // ========== CONFIGURACIÓN DE CATEGORIA ==========

            builder.Entity<Categoria>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Categorias)
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
