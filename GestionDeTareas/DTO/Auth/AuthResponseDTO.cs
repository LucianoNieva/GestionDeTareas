namespace GestionDeTareas.DTO.Auth
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public DateTime Expiracion { get; set; }
    }
}
