using System.ComponentModel.DataAnnotations;

namespace GestionDeTareas.DTO.Category
{
    public class CategoryCreacionDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚ\s]+$", ErrorMessage = "El nombre solo puede contener letras, números y espacios")]
        public string Nombre { get; set; } = null!;  
    }
}
