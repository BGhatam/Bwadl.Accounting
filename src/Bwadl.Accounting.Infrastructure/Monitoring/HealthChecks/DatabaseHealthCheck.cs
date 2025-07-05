using Microsoft.Extensions.Diagnostics.HealthChecks;
using Bwadl.Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bwadl.Accounting.Infrastructure.Monitoring.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;

    public DatabaseHealthCheck(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            
            var data = new Dictionary<string, object>
            {
                { "database", _context.Database.GetDbConnection().Database ?? "Unknown" },
                { "provider", _context.Database.ProviderName ?? "Unknown" }
            };

            // Report metric to Prometheus
            PrometheusMetrics.HealthCheckStatus.WithLabels("database").Set(1);
            
            return HealthCheckResult.Healthy("Database connection is healthy", data);
        }
        catch (Exception ex)
        {
            // Report failure metric to Prometheus
            PrometheusMetrics.HealthCheckStatus.WithLabels("database").Set(0);
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}
