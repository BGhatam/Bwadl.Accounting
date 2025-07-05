using Bwadl.Accounting.Domain.Common;

namespace Bwadl.Accounting.Domain.Entities;

public class ApiKey : IVersionedEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string KeyHash { get; private set; } = null!;
    public string? Description { get; private set; }
    public int? UserId { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? LastUsedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsRevoked { get; private set; } = false;
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public string? RevokedReason { get; private set; }
    
    // Rate limiting
    public int? RateLimitPerMinute { get; private set; }
    public int? RateLimitPerHour { get; private set; }
    public int? RateLimitPerDay { get; private set; }
    
    // IP restrictions
    public string? AllowedIpAddresses { get; private set; } // JSON array of IP addresses
    
    // Permissions
    public string? Permissions { get; private set; } // JSON array of permission names
    
    // IVersionedEntity implementation
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = null!;

    // Navigation properties
    public User? User { get; private set; }

    // Private constructor for EF Core
    private ApiKey() { }

    // Public constructor
    public ApiKey(string name, string keyHash, string? description = null, int? userId = null, DateTime? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("API key name cannot be empty", nameof(name));
        }
        
        if (string.IsNullOrWhiteSpace(keyHash))
        {
            throw new ArgumentException("API key hash cannot be empty", nameof(keyHash));
        }

        Name = name;
        KeyHash = keyHash;
        Description = description;
        UserId = userId;
        ExpiresAt = expiresAt;
        IsActive = true;
        IsRevoked = false;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("API key name cannot be empty", nameof(name));
        }
        
        Name = name;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void SetExpiration(DateTime? expiresAt)
    {
        ExpiresAt = expiresAt;
    }

    public void SetRateLimits(int? perMinute = null, int? perHour = null, int? perDay = null)
    {
        RateLimitPerMinute = perMinute;
        RateLimitPerHour = perHour;
        RateLimitPerDay = perDay;
    }

    public void SetAllowedIpAddresses(string? allowedIpAddresses)
    {
        AllowedIpAddresses = allowedIpAddresses;
    }

    public void SetPermissions(string? permissions)
    {
        Permissions = permissions;
    }

    public void RecordUsage()
    {
        LastUsedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsRevoked)
        {
            throw new InvalidOperationException("Cannot activate a revoked API key");
        }
        
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Revoke(string revokedBy, string? reason = null)
    {
        IsRevoked = true;
        IsActive = false;
        RevokedAt = DateTime.UtcNow;
        RevokedBy = revokedBy;
        RevokedReason = reason;
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    
    public bool IsValidKey => IsActive && !IsRevoked && !IsExpired;
}
