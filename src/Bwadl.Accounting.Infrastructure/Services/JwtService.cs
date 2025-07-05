using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Bwadl.Accounting.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
    }

    public TimeSpan AccessTokenExpiration => TimeSpan.FromMinutes(_jwtSettings.AccessTokenExpirationMinutes);
    public TimeSpan RefreshTokenExpiration => TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays);

    public string GenerateToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.NameEn ?? user.NameAr ?? user.Email ?? user.Mobile?.ToString() ?? "Unknown"),
        };

        // Add email if available
        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        // Add roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add permissions as custom claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        // Add verification status
        claims.Add(new Claim("email_verified", user.IsEmailVerified.ToString().ToLower()));
        claims.Add(new Claim("mobile_verified", user.IsMobileVerified.ToString().ToLower()));
        claims.Add(new Claim("user_verified", user.IsUserVerified.ToString().ToLower()));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(AccessTokenExpiration),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = _signingKey,
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = _jwtSettings.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(_jwtSettings.ClockSkewMinutes)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            // Additional validation to ensure it's a JWT token with the expected algorithm
            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(_jwtSettings.Algorithm, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public AccessTokenResult GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var token = GenerateToken(user, roles, permissions);
        var expiresAt = DateTime.UtcNow.Add(AccessTokenExpiration);
        return new AccessTokenResult(token, expiresAt);
    }

    public Domain.Interfaces.TokenValidationResult ValidateAccessToken(string token)
    {
        try
        {
            var principal = ValidateToken(token);
            if (principal == null)
            {
                return new Domain.Interfaces.TokenValidationResult(false, null, null);
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var expClaim = principal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
            
            DateTime? expiresAt = null;
            if (long.TryParse(expClaim, out var exp))
            {
                expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            }

            return new Domain.Interfaces.TokenValidationResult(true, userIdClaim, expiresAt);
        }
        catch
        {
            return new Domain.Interfaces.TokenValidationResult(false, null, null);
        }
    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        // For basic implementation, we just check if it's not null/empty
        // In a production environment, you might store refresh tokens in database
        // and validate against that store
        return !string.IsNullOrWhiteSpace(refreshToken);
    }
}
