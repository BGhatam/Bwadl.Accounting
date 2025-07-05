using Bwadl.Accounting.Domain.Entities;
using System.Security.Claims;

namespace Bwadl.Accounting.Domain.Interfaces;

public record AccessTokenResult(string Token, DateTime ExpiresAt);
public record TokenValidationResult(bool IsValid, string? UserId, DateTime? ExpiresAt);

public interface IJwtService
{
    /// <summary>
    /// Generates a JWT access token for the specified user
    /// </summary>
    /// <param name="user">The user to generate the token for</param>
    /// <param name="roles">The user's roles</param>
    /// <param name="permissions">The user's permissions</param>
    /// <returns>The generated JWT access token with expiration</returns>
    AccessTokenResult GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);

    /// <summary>
    /// Generates a JWT token for the specified user (backward compatibility)
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
    /// Validates a JWT access token and returns validation result
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>Token validation result</returns>
    TokenValidationResult ValidateAccessToken(string token);

    /// <summary>
    /// Validates a refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool ValidateRefreshToken(string refreshToken);

    /// <summary>
    /// Validates a JWT token and returns the claims (backward compatibility)
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
