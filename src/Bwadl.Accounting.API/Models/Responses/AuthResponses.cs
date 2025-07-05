namespace Bwadl.Accounting.API.Models.Responses;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserResponse User { get; set; } = null!;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public IEnumerable<string> Permissions { get; set; } = new List<string>();
}

public class TokenValidationResponse
{
    public bool IsValid { get; set; }
    public string? UserId { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public IEnumerable<string> Permissions { get; set; } = new List<string>();
    public DateTime? ExpiresAt { get; set; }
}

public class ApiKeyResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ApiKey { get; set; } // Only returned when creating a new key
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsRevoked { get; set; }
    public int? RateLimitPerMinute { get; set; }
    public int? RateLimitPerHour { get; set; }
    public int? RateLimitPerDay { get; set; }
    public IEnumerable<string>? AllowedIpAddresses { get; set; }
    public IEnumerable<string>? Permissions { get; set; }
}
