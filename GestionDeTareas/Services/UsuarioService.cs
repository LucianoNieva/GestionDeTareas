using GestionDeTareas.Datos;
using GestionDeTareas.DTO.Auth;
using GestionDeTareas.DTO.Usuario;
using GestionDeTareas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UsuarioService : IUsuarioService
{
    private readonly UserManager<Usuario> _userManager;
    private readonly ApplicationDbContext _context;

    public UsuarioService(UserManager<Usuario> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // ========== CONSULTAS ==========

    public async Task<bool> ExisteUsuarioAsync(string userId)
    {
        var usuario = await _userManager.FindByIdAsync(userId);
        return usuario != null;
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<List<UsuarioDTO>> GetUsuariosDisponiblesAsync()
    {
        return await _context.Users
            .Select(u => new UsuarioDTO
            {
                Id = u.Id,
                NombreApellido = u.Nombre,
                Email = u.Email ?? string.Empty,
                // Esto EF lo convierte en un JOIN con la tabla de roles
                Roles = _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles,
                          ur => ur.RoleId,
                          r => r.Id,
                          (ur, r) => r.Name!)
                    .ToList()
            })
            .ToListAsync();
    }

    // ========== ACCIONES DE MANTENIMIENTO ==========

    public async Task<IdentityResult> CrearUsuarioConRolAsync(UsuarioCreacionDTO dto, string rol)
    {
        var usuario = new Usuario
        {
            UserName = dto.Email,
            Email = dto.Email,
            Nombre = dto.Nombre
        };

        var resultado = await _userManager.CreateAsync(usuario, dto.Password);

        if (resultado.Succeeded)
        {
            await _userManager.AddToRoleAsync(usuario, rol);
        }

        return resultado;
    }

    public async Task<IdentityResult> CambiarRolAsync(string userId, string nuevoRol)
    {
        var usuario = await _userManager.FindByIdAsync(userId);
        if (usuario == null)
            return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });

        var rolesActuales = await _userManager.GetRolesAsync(usuario);

        // Limpiamos roles viejos y ponemos el nuevo
        await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
        return await _userManager.AddToRoleAsync(usuario, nuevoRol);
    }

    public async Task<IdentityResult> EliminarUsuarioAsync(string userId)
    {
        var usuario = await _userManager.FindByIdAsync(userId);
        if (usuario == null)
            return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });

        return await _userManager.DeleteAsync(usuario);
    }

    public async Task<(bool esValido, Usuario? usuario, bool estaBloqueado)> ValidarPasswordAsync(string email, string password)
    {
        var usuario = await _userManager.FindByEmailAsync(email);
        if (usuario == null) return (false, null, false);

        // Verificar si está bloqueado
        if (await _userManager.IsLockedOutAsync(usuario))
            return (false, usuario, true);

        var resultado = await _userManager.CheckPasswordAsync(usuario, password);

        if (resultado)
        {
            await _userManager.ResetAccessFailedCountAsync(usuario);
            return (true, usuario, false);
        }
        else
        {
            await _userManager.AccessFailedAsync(usuario);
            return (false, usuario, false);
        }
    }

    public async Task<IdentityResult> RegistrarUsuarioAsync(RegisterDTO dto, string rol)
    {
        var usuario = new Usuario
        {
            UserName = dto.Email,
            Email = dto.Email,
            Nombre = dto.Nombre
        };

        var resultado = await _userManager.CreateAsync(usuario, dto.Password);

        if (resultado.Succeeded)
        {
            await _userManager.AddToRoleAsync(usuario, rol);
        }

        return resultado;
    }
}