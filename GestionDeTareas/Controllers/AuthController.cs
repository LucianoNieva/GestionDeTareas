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
        private readonly UserManager<Usuario> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<Usuario> userManager,
            ITokenService tokenService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

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

                var token = _tokenService.GenerarToken(usuario.Email!, usuario.Id);

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


        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO dto)
        {
            try
            {
                var usuario = await _userManager.FindByEmailAsync(dto.Email);
                if (usuario == null)
                {
                    return Unauthorized();
                }

                var esValido = await _userManager.CheckPasswordAsync(usuario, dto.Password);
                if (!esValido)
                {
                    await _userManager.AccessFailedAsync(usuario);

                    if (await _userManager.IsLockedOutAsync(usuario))
                    {
                        _logger.LogWarning("Usuario bloqueado temporalmente: {Email}", usuario.Email);
                        return Unauthorized();
                    }

                    return Unauthorized();
                }

                await _userManager.ResetAccessFailedCountAsync(usuario);

                var token = _tokenService.GenerarToken(usuario.Email!, usuario.Id);

                _logger.LogInformation("Usuario inició sesión: {Email}", usuario.Email);

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
                _logger.LogError(ex, "Error al iniciar sesión");
                return StatusCode(500);
            }
        }
    }
}
