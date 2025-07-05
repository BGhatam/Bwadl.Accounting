using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bwadl.Accounting.Infrastructure.Data;
using System;

namespace Bwadl.Accounting.Tests.Integration;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            // Clear existing configuration sources
            config.Sources.Clear();
            
            // Add test configuration
            config.AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: false);
            
            // Add in-memory configuration for testing - specifically remove any connection string
            // so the infrastructure will default to InMemory database
            var testConfig = new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Testing",
                ["ConnectionStrings:DefaultConnection"] = "", // Force empty to use InMemory in infrastructure
                ["Features:EnableCaching"] = "false",
                ["Features:EnableEmailNotifications"] = "false",
                ["Features:EnableEventDrivenArchitecture"] = "false",
                ["Features:EnableRateLimiting"] = "false",
                ["Security:ApiKeys:RequireApiKey"] = "false",
                ["HealthChecks:CheckRedis"] = "false",
                ["HealthChecks:CheckMessageBus"] = "false"
            };
            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureServices(services =>
        {
            // Configure logging for tests
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Warning);
                builder.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            });
        });

        builder.UseEnvironment("Testing");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                using var scope = Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                // Only try to clean up if it's an in-memory database
                if (Microsoft.EntityFrameworkCore.InMemoryDatabaseFacadeExtensions.IsInMemory(context.Database))
                {
                    context.Database.EnsureDeleted();
                }
            }
            catch (ObjectDisposedException)
            {
                // Service provider already disposed, nothing to clean up
            }
            catch (Exception)
            {
                // Ignore cleanup errors in tests
            }
        }
        base.Dispose(disposing);
    }
}
