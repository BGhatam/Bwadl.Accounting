using Bwadl.Accounting.Domain.Entities;
using System.Security.Claims;

namespace Bwadl.Accounting.Domain.Interfaces;

public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the specified user
    /// </summary>
    /// <param name="user">The user to generate the token for</param>
    /// <param name="roles">The user's roles</param>
    /// <param name="permissions">The user's permissions</param>
    /// <returns>The generated JWT token</returns>
    string GenerateToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <returns>A refresh token</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT token and returns the claims
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>The claims principal if valid, null otherwise</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Gets the expiration time for access tokens
    /// </summary>
    TimeSpan AccessTokenExpiration { get; }

    /// <summary>
    /// Gets the expiration time for refresh tokens
    /// </summary>
    TimeSpan RefreshTokenExpiration { get; }
}
