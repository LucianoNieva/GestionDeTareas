using GestionDeTareas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionDeTareas.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Usuario> _userManager;

        public TokenService(IConfiguration configuration, UserManager<Usuario> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerarToken(Usuario usuario)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id),
                new Claim(ClaimTypes.Email,usuario.Email!),
                new Claim(ClaimTypes.Name, usuario.Nombre)
            };

            var roles = await _userManager.GetRolesAsync(usuario);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.AddDays(7),
               signingCredentials: credenciales
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
