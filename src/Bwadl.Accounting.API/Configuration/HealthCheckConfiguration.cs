using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Bwadl.Accounting.API.Configuration;

/// <summary>
/// API-layer configuration for health check HTTP endpoints and UI.
/// The actual health check implementations are in the Infrastructure layer.
/// </summary>
public static class HealthCheckConfiguration
{
    /// <summary>
    /// Configures health check UI components.
    /// Health check services themselves are registered in Infrastructure layer.
    /// </summary>
    public static IServiceCollection AddHealthCheckUI(this IServiceCollection services, IConfiguration configuration)
    {
        // Only UI configuration in API layer - health checks registered in Infrastructure
        services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(15); // More frequent updates
            options.SetMinimumSecondsBetweenFailureNotifications(60);
            // Configure endpoints for Kubernetes probes
            options.AddHealthCheckEndpoint("Liveness", "/health/live");
            options.AddHealthCheckEndpoint("Readiness", "/health/ready");
            options.AddHealthCheckEndpoint("All Checks", "/health");
            options.SetHeaderText("Bwadl API Health Dashboard");
        })
        .AddInMemoryStorage(); // This will reset the storage

        return services;
    }

    public static IApplicationBuilder UseHealthCheckConfiguration(this IApplicationBuilder app)
    {        
        // Liveness probe for Kubernetes (basic functionality)
        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        // Readiness probe for Kubernetes (external dependencies)
        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });
        
        // General health check endpoint (all checks)
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });
        
        // Detailed health check endpoint  
        app.UseHealthChecks("/health/detailed", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(x => new
                    {
                        name = x.Key,
                        status = x.Value.Status.ToString(),
                        description = x.Value.Description,
                        duration = x.Value.Duration.ToString(),
                        data = x.Value.Data,
                        tags = x.Value.Tags
                    }),
                    totalDuration = report.TotalDuration.ToString()
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        });
        
        // Health checks API endpoint for UI
        app.UseHealthChecks("/health-api", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        
        // Health Checks UI with explicit configuration
        app.UseHealthChecksUI(config =>
        {
            config.UIPath = "/health-ui";
            config.ApiPath = "/health-api";
            config.WebhookPath = "/health-webhooks";
            config.ResourcesPath = "/health-ui-resources";
            config.AsideMenuOpened = true;
            config.PageTitle = "Bwadl API Health Checks";
        });

        return app;
    }
}
