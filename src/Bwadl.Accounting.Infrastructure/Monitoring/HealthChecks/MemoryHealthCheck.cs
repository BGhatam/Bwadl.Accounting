using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bwadl.Accounting.Infrastructure.Monitoring.HealthChecks;

public class MemoryHealthCheck : IHealthCheck
{
    private const long MaxMemoryBytes = 1_000_000_000; // 1GB threshold
    private const long WarningMemoryBytes = 750_000_000; // 750MB warning threshold

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        var allocated = GC.GetTotalMemory(false);
        var data = new Dictionary<string, object>
        {
            { "allocated_bytes", allocated },
            { "allocated_mb", allocated / 1024 / 1024 },
            { "gen0_collections", GC.CollectionCount(0) },
            { "gen1_collections", GC.CollectionCount(1) },
            { "gen2_collections", GC.CollectionCount(2) },
            { "max_threshold_bytes", MaxMemoryBytes },
            { "warning_threshold_bytes", WarningMemoryBytes }
        };

        HealthStatus status;
        string description;

        if (allocated >= MaxMemoryBytes)
        {
            status = HealthStatus.Unhealthy;
            description = $"Memory usage is critical: {allocated:N0} bytes (>{MaxMemoryBytes:N0})";
            PrometheusMetrics.HealthCheckStatus.WithLabels("memory").Set(0);
        }
        else if (allocated >= WarningMemoryBytes)
        {
            status = HealthStatus.Degraded;
            description = $"Memory usage is high: {allocated:N0} bytes (>{WarningMemoryBytes:N0})";
            PrometheusMetrics.HealthCheckStatus.WithLabels("memory").Set(0.5);
        }
        else
        {
            status = HealthStatus.Healthy;
            description = $"Memory usage is normal: {allocated:N0} bytes";
            PrometheusMetrics.HealthCheckStatus.WithLabels("memory").Set(1);
        }

        return Task.FromResult(new HealthCheckResult(status, description, data: data));
    }
}
