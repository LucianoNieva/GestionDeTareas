using System.Security.Claims;

namespace GestionDeTareas.Services
{
    /// <summary>
    /// Implementación del servicio de usuario actual
    /// Proporciona acceso thread-safe a la información del usuario autenticado
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Obtiene el ClaimsPrincipal del usuario actual
        /// </summary>
        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User
            ?? throw new InvalidOperationException("No hay contexto HTTP disponible");

        /// <inheritdoc/>
        public string UserId
        {
            get
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("Usuario no autenticado o UserId no disponible");

                return userId;
            }
        }

        /// <inheritdoc/>
        public string Email
        {
            get
            {
                var email = User.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(email))
                    throw new UnauthorizedAccessException("Email no disponible en claims");

                return email;
            }
        }

        /// <inheritdoc/>
        public string Name => User.FindFirstValue(ClaimTypes.Name)
            ?? User.FindFirstValue(ClaimTypes.Email)
            ?? "Usuario Desconocido";

        /// <inheritdoc/>
        public bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;

        /// <inheritdoc/>
        public bool IsInRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("El rol no puede ser nulo o vacío", nameof(role));

            return User.IsInRole(role);
        }

        /// <inheritdoc/>
        public IEnumerable<string> Roles => User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct();

        /// <inheritdoc/>
        public string? GetClaimValue(string claimType)
        {
            if (string.IsNullOrWhiteSpace(claimType))
                throw new ArgumentException("El tipo de claim no puede ser nulo o vacío", nameof(claimType));

            return User.FindFirstValue(claimType);
        }
    }
}
