
using System.ComponentModel.DataAnnotations;

namespace GestionDeTareas.Models
{
    
    public class Tarea
    {
        public int Id { get; set; }

        public required string Titulo { get; set; }

        public Estado Estado { get; set; } = Estado.Pendiente;
        public Prioridad Prioridad{ get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public string IdAdmin { get; set; } = null!;
        public Usuario Admin { get; set; } = null!;
        public string? IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }
        public int? IdCategoria { get; set; }
        public Categoria? Categoria { get; set; }


    }

    public enum Prioridad
    {
        Baja = 1,
        Alta = 2
    };

    public enum Estado
    {
        Pendiente = 1,
        EnProceso = 2,
        Terminado = 3
    };


}
