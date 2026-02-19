namespace GestionDeTareas.DTO.Usuario
{
    public class UsuarioCreacionDTO
    {
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
