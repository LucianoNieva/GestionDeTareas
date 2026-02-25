using System.ComponentModel.DataAnnotations;

namespace GestionDeTareas.DTO.Usuario
{
    public class UsuarioCreacionDTO
    {
        public required string Nombre { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        [Required]
        [RegularExpression("^(Admin|Usuario)$", ErrorMessage = "El rol debe ser 'Admin' o 'Usuario'")]
        public string Rol { get; set; } = "Usuario";
    }
}
