namespace GestionDeTareas.DTO.Usuario
{
    public class UsuarioDTO
    {
        public string? Id { get; set; }
        public string? NombreApellido { get; set; }
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = new();

    }
}
