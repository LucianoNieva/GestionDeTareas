using Microsoft.AspNetCore.Identity;

namespace GestionDeTareas.Models
{
    public class Usuario : IdentityUser
    {
        public string Nombre { get; set; } = null!;
        public string? Apellido { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public List<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}
