using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bwadl.Accounting.Infrastructure.Monitoring.HealthChecks;

public class SelfHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>
        {
            { "uptime", Environment.TickCount64 },
            { "environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown" },
            { "machine_name", Environment.MachineName },
            { "process_id", Environment.ProcessId },
            { "working_set", Environment.WorkingSet }
        };

        return Task.FromResult(HealthCheckResult.Healthy("API is running", data));
    }
}
