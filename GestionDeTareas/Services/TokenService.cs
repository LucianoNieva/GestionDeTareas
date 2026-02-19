using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionDeTareas.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerarToken(string email, string userId)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email,email)
            };

            var llave = _config["Jwt:Key"];

            if (string.IsNullOrEmpty(llave))
            {
                throw new InvalidOperationException("La llave JWT no está configurada");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(llave));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiracion,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
