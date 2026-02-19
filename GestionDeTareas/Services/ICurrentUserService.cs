namespace GestionDeTareas.Services
{
    /// <summary>
    /// Servicio para acceder a la información del usuario autenticado actual
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Obtiene el ID del usuario autenticado
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Obtiene el email del usuario autenticado
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Obtiene el nombre del usuario autenticado
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indica si el usuario está autenticado
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Verifica si el usuario pertenece a un rol específico
        /// </summary>
        bool IsInRole(string role);

        /// <summary>
        /// Obtiene todos los roles del usuario
        /// </summary>
        IEnumerable<string> Roles { get; }

        /// <summary>
        /// Obtiene un claim específico por su tipo
        /// </summary>
        string? GetClaimValue(string claimType);
    }
}
