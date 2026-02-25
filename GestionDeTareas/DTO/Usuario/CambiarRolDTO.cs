using System.ComponentModel.DataAnnotations;

namespace GestionDeTareas.DTO.Usuario
{
    public class CambiarRolDTO
    {
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [RegularExpression("^(Admin|Usuario)$", ErrorMessage = "El rol debe ser 'Admin' o 'Usuario'")]
        public string NuevoRol { get; set; } = null!;
    }
}
