namespace Bwadl.Accounting.Shared.Configuration;

public class SecurityOptions
{
    public const string SectionName = "Security";
    
    public JwtOptions Jwt { get; set; } = new();
    public ApiKeyOptions ApiKeys { get; set; } = new();
}

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty; // Will be retrieved from secrets
    public int AccessTokenExpirationMinutes { get; set; } = 60; // 1 hour
    public int RefreshTokenExpirationDays { get; set; } = 7; // 7 days
    public string Algorithm { get; set; } = "HS256";
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public int ClockSkewMinutes { get; set; } = 5;
}

public class ApiKeyOptions
{
    public bool RequireApiKey { get; set; } = false;
    public List<string> ValidApiKeys { get; set; } = new();
    public int KeyLength { get; set; } = 32;
    public string Prefix { get; set; } = "bwa_";
    public bool RequireHttps { get; set; } = true;
    public int DefaultRateLimitPerMinute { get; set; } = 100;
    public int DefaultRateLimitPerHour { get; set; } = 1000;
    public int DefaultRateLimitPerDay { get; set; } = 10000;
    public bool EnableIpWhitelisting { get; set; } = true;
    public string HeaderName { get; set; } = "X-API-Key";
}
