using Prometheus;

namespace Bwadl.Accounting.Infrastructure.Monitoring;

public static class PrometheusMetrics
{
    // Custom business metrics
    public static readonly Counter UsersCreated = Metrics
        .CreateCounter("bwadl_users_created_total", "Total number of users created");

    public static readonly Counter AuthenticationAttempts = Metrics
        .CreateCounter("bwadl_auth_attempts_total", "Total authentication attempts", new[] { "result" });

    public static readonly Histogram RequestDuration = Metrics
        .CreateHistogram("bwadl_request_duration_seconds", "Request duration in seconds", 
        new[] { "method", "endpoint", "status_code" });

    public static readonly Gauge ActiveConnections = Metrics
        .CreateGauge("bwadl_active_connections", "Number of active connections");

    public static readonly Counter EmailsSent = Metrics
        .CreateCounter("bwadl_emails_sent_total", "Total emails sent", new[] { "type", "status" });

    // Health check metrics
    public static readonly Gauge HealthCheckStatus = Metrics
        .CreateGauge("bwadl_health_check_status", "Health check status (1=healthy, 0=unhealthy)", 
        new[] { "check_name" });

    // Currency operations
    public static readonly Counter CurrencyOperations = Metrics
        .CreateCounter("bwadl_currency_operations_total", "Total currency operations", new[] { "operation" });

    // API endpoint specific metrics
    public static readonly Counter ApiRequests = Metrics
        .CreateCounter("bwadl_api_requests_total", "Total API requests", new[] { "controller", "action", "method" });

    public static readonly Histogram ApiRequestDuration = Metrics
        .CreateHistogram("bwadl_api_request_duration_seconds", "API request duration", 
        new[] { "controller", "action" });

    // Database metrics
    public static readonly Counter DatabaseQueries = Metrics
        .CreateCounter("bwadl_database_queries_total", "Total database queries", new[] { "entity", "operation" });

    public static readonly Histogram DatabaseQueryDuration = Metrics
        .CreateHistogram("bwadl_database_query_duration_seconds", "Database query duration", 
        new[] { "entity", "operation" });

    // Cache metrics
    public static readonly Counter CacheOperations = Metrics
        .CreateCounter("bwadl_cache_operations_total", "Total cache operations", new[] { "operation", "result" });

    // JWT Token metrics
    public static readonly Counter JwtTokensGenerated = Metrics
        .CreateCounter("bwadl_jwt_tokens_generated_total", "Total JWT tokens generated", new[] { "type" });

    public static readonly Counter JwtTokensValidated = Metrics
        .CreateCounter("bwadl_jwt_tokens_validated_total", "Total JWT tokens validated", new[] { "result" });
}
