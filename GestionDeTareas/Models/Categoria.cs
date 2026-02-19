using System.ComponentModel.DataAnnotations;

namespace GestionDeTareas.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        public required string Nombre { get; set; }

        public string IdUsuario { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;

        public List<Tarea>? TareasEnCategoria { get; set; } = new List<Tarea>();

        
    }
}
