using GestionDeTareas.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace GestionDeTareas.DTO.Tarea
{
    public class ActualizarTareaDTO
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200)]
        public string? Titulo { get; set; } = null!;
        public string? Estado { get; set; }
        public string? Prioridad { get; set; }

        public int? IdCategoria { get; set; }
    }
}
