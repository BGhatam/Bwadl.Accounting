namespace Bwadl.Accounting.Domain.Settings;

public class ApiKeySettings
{
    public const string SectionName = "ApiKeySettings";
    
    public int KeyLength { get; set; } = 32;
    public string Prefix { get; set; } = "bwa_";
    public bool RequireHttps { get; set; } = true;
    public int DefaultRateLimitPerMinute { get; set; } = 100;
    public int DefaultRateLimitPerHour { get; set; } = 1000;
    public int DefaultRateLimitPerDay { get; set; } = 10000;
    public bool EnableIpWhitelisting { get; set; } = true;
    public string HeaderName { get; set; } = "X-API-Key";
}
