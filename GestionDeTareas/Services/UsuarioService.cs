using GestionDeTareas.DTO.Usuario;
using GestionDeTareas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionDeTareas.Services
{
    public class UsuarioService: IUsuarioService
    {
        private readonly UserManager<Usuario> _userManager;

        public UsuarioService(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ExisteUsuarioAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId) != null;
        }

        public async Task<List<UsuarioDTO>> GetUsuariosDisponiblesAsync()
        {
            // Traemos los usuarios de la base de datos
            var usuarios = await _userManager.Users.ToListAsync();
            var usuariosDTO = new List<UsuarioDTO>();

            foreach (var usuario in usuarios)
            {
                // Obtenemos los roles de cada usuario
                var roles = await _userManager.GetRolesAsync(usuario);

                usuariosDTO.Add(new UsuarioDTO
                {
                    Id = usuario.Id,
                    NombreApellido = usuario.Nombre,
                    Email = usuario.Email ?? string.Empty,
                    Roles = roles.ToList()
                });
            }

            return usuariosDTO;
        }
    
    }
}
