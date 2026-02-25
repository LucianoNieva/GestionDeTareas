using GestionDeTareas.Datos;
using GestionDeTareas.DTO.Auth;
using GestionDeTareas.Models;
using GestionDeTareas.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;

namespace GestionDeTareas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUsuarioService usuarioService, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _usuarioService = usuarioService;
            _tokenService = tokenService;
            _logger = logger;
        }
        /*
        [HttpPost("registrar")]
        public async Task<ActionResult<AuthResponseDTO>> Registrar(RegisterDTO registerDTO)
        {
            try
            {
                var usuarioExiste = await _userManager.FindByEmailAsync(registerDTO.Email);

                if (usuarioExiste != null) return BadRequest("El email ya esta registrado");

                var usuario = new Usuario
                {

                    UserName = registerDTO.Email,
                    Email = registerDTO.Email,
                    Nombre = registerDTO.Nombre
                };

                var resultado = await _userManager.CreateAsync(usuario, registerDTO.Password);

                if (!resultado.Succeeded) return BadRequest(resultado.Errors);

                await _userManager.AddToRoleAsync(usuario, "Usuario");

                var token = await _tokenService.GenerarToken(usuario);

                _logger.LogInformation($"Usuario registrado exitosamente: {usuario.Email}");

                return Ok(new AuthResponseDTO
                {
                    Token = token,
                    Email = usuario.Email!,
                    Nombre = usuario.Nombre,
                    Expiracion = DateTime.UtcNow.AddDays(7)
                });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error al registrar usuario");
                return StatusCode(500);
            
            }


        }*/

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO dto)
        {
            var (esValido, usuario, estaBloqueado) = await _usuarioService.ValidarPasswordAsync(dto.Email, dto.Password);

            if (estaBloqueado)
                return Unauthorized(new { message = "Cuenta bloqueada temporalmente por demasiados intentos fallidos." });

            if (!esValido || usuario == null)
                return Unauthorized(new { message = "Credenciales incorrectas" });

            var token = await _tokenService.GenerarToken(usuario);

            _logger.LogInformation("Usuario inició sesión: {Email}", usuario.Email);

            return Ok(new AuthResponseDTO
            {
                Token = token,
                Email = usuario.Email!,
                Nombre = usuario.Nombre,
                Expiracion = DateTime.UtcNow.AddDays(7)
            });
        }

        [HttpPost("setup/roles")]
        public async Task<ActionResult> CrearRoles(
            [FromServices] RoleManager<IdentityRole> roleManager)
        {
            try
            {
                // Crear rol Admin
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Crear rol Usuario
                if (!await roleManager.RoleExistsAsync("Usuario"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Usuario"));
                }

                return Ok(new
                {
                    message = "Roles creados exitosamente",
                    roles = new[] { "Admin", "Usuario" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear roles");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("setup/crear-admin")]
        public async Task<ActionResult<AuthResponseDTO>> CrearPrimerAdmin(RegisterDTO dto)
        {
            var resultado = await _usuarioService.RegistrarUsuarioAsync(dto, "Admin");

            if (!resultado.Succeeded)
                return BadRequest(new { message = "Error al crear admin", errores = resultado.Errors.Select(e => e.Description) });

            var usuario = await _usuarioService.BuscarPorEmailAsync(dto.Email);
            var token = await _tokenService.GenerarToken(usuario!);

            return Ok(new AuthResponseDTO { Token = token, Email = usuario!.Email!, Nombre = usuario.Nombre });
        }
    }
}
