using GestionDeTareas.DTO.Auth;
using GestionDeTareas.DTO.Usuario;
using GestionDeTareas.Models;
using Microsoft.AspNetCore.Identity;

public interface IUsuarioService
{
    // Consultas
    Task<List<UsuarioDTO>> GetUsuariosDisponiblesAsync();
    Task<bool> ExisteUsuarioAsync(string userId);
    Task<Usuario?> BuscarPorEmailAsync(string email);

    // NUEVOS METODOS PARA AUTH
    Task<(bool esValido, Usuario? usuario, bool estaBloqueado)> ValidarPasswordAsync(string email, string password);
    Task<IdentityResult> RegistrarUsuarioAsync(RegisterDTO dto, string rol);

    // Acciones de Admin
    Task<IdentityResult> CrearUsuarioConRolAsync(UsuarioCreacionDTO dto, string rol);
    Task<IdentityResult> CambiarRolAsync(string userId, string nuevoRol);
    Task<IdentityResult> EliminarUsuarioAsync(string userId);
}