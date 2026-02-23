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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión");
                return StatusCode(500);
            }
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
            try
            {
                var usuarioExiste = await _userManager.FindByEmailAsync(dto.Email);
                if (usuarioExiste != null)
                    return BadRequest(new { message = "El email ya está registrado" });

                var usuario = new Usuario
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    Nombre = dto.Nombre
                };

                var resultado = await _userManager.CreateAsync(usuario, dto.Password);

                if (!resultado.Succeeded)
                {
                    var errores = resultado.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "Error al crear usuario", errores });
                }

                // ✅ ASIGNAR ROL ADMIN
                await _userManager.AddToRoleAsync(usuario, "Admin");

                var token = await _tokenService.GenerarToken(usuario);

                _logger.LogInformation("ADMIN creado exitosamente: {Email}", usuario.Email);

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
                _logger.LogError(ex, "Error al crear admin");
                return StatusCode(500, new { message = "Error al crear admin" });
            }
        }
    }
}
