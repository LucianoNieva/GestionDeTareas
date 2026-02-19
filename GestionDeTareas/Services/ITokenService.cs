namespace GestionDeTareas.Services
{
    public interface ITokenService
    {
        string GenerarToken(string email, string userId);
    }
}