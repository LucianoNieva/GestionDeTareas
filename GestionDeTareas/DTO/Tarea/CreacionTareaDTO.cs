using GestionDeTareas.Models;
using System.ComponentModel.DataAnnotations;

namespace GestionDeTareas.DTO.Tarea
{
    public class CreacionTareaDTO
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede superar 200 caracteres")]
        public string Titulo { get; set; } = null!;
        public string? Prioridad { get; set; }
        public int? IdCategoria { get; set; }

    }
}
