using GestionDeTareas.Models;

namespace GestionDeTareas.Services
{
    public interface ITokenService
    {
        Task<string> GenerarToken(Usuario usuario);
    }
}