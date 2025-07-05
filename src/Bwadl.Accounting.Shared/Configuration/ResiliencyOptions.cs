namespace Bwadl.Accounting.Shared.Configuration;

/// <summary>
/// Configuration for application resiliency patterns including retry policies
/// </summary>
public class ResiliencyOptions
{
    public const string SectionName = "Resiliency";
    
    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    public int RetryCount { get; set; } = 3;
    
    /// <summary>
    /// Base delay in milliseconds for exponential backoff
    /// </summary>
    public int BaseDelayMs { get; set; } = 1000;
    
    /// <summary>
    /// Maximum delay between retries in milliseconds
    /// </summary>
    public int MaxDelayMs { get; set; } = 30000;
    
    /// <summary>
    /// Global toggle to enable/disable retry functionality
    /// </summary>
    public bool EnableRetry { get; set; } = true;
    
    /// <summary>
    /// Exception types that should trigger retry attempts
    /// </summary>
    public string[] RetriableExceptions { get; set; } = new[]
    {
        "System.Data.SqlClient.SqlException",
        "System.TimeoutException",
        "System.Net.Http.HttpRequestException",
        "Microsoft.EntityFrameworkCore.DbUpdateException",
        "Npgsql.NpgsqlException"
    };
    
    /// <summary>
    /// Command/Query types that should never be retried
    /// </summary>
    public string[] NonRetriableOperations { get; set; } = new[]
    {
        "DeleteUserCommand",
        "DeleteCurrencyCommand",
        "ChangePasswordCommand",
        "LoginCommand",
        "RegisterCommand"
    };
}
