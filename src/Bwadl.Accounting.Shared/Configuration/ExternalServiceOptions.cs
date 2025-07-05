namespace Bwadl.Accounting.Shared.Configuration;

public class ExternalServiceOptions
{
    public const string SectionName = "ExternalServices";
    
    public EmailServiceOptions EmailService { get; set; } = new();
}

public class EmailServiceOptions
{
    public string Provider { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryAttempts { get; set; } = 3;
}
