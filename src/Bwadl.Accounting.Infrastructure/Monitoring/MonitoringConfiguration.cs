using Bwadl.Accounting.Infrastructure.Monitoring.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Bwadl.Accounting.Infrastructure.Monitoring;

public static class MonitoringConfiguration
{
    public static IServiceCollection AddMonitoring(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register health check services
        services.AddScoped<DatabaseHealthCheck>();
        services.AddScoped<CacheHealthCheck>();
        services.AddScoped<MemoryHealthCheck>();
        services.AddScoped<SelfHealthCheck>();

        // Configure health checks with proper tags for Kubernetes probes
        services.AddHealthChecks()
            // Liveness probes - basic functionality
            .AddCheck<SelfHealthCheck>("self", tags: new[] { "live" })
            .AddCheck<MemoryHealthCheck>("memory", tags: new[] { "live" })
            
            // Readiness probes - external dependencies
            .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready", "live" })
            .AddCheck<CacheHealthCheck>("cache", tags: new[] { "ready" });

        return services;
    }
}
