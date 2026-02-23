
using GestionDeTareas.DTO.Usuario;

namespace GestionDeTareas.Services
{
    public interface IUsuarioService
    {
        Task<bool> ExisteUsuarioAsync(string userId);
        Task<List<UsuarioDTO>> GetUsuariosDisponiblesAsync();
    }
}
