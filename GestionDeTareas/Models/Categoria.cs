using System.ComponentModel.DataAnnotations;

namespace GestionDeTareas.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public required string Nombre { get; set; }

        public List<Tarea>? TareasEnCategoria { get; set; } = new List<Tarea>();

        
    }
}
