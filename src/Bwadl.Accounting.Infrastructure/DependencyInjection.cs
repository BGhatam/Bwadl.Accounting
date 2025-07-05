using Bwadl.Accounting.Application.Common.Interfaces;
using Bwadl.Accounting.Shared.Configuration;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Infrastructure.Caching;
using Bwadl.Accounting.Infrastructure.Configuration;
using Bwadl.Accounting.Infrastructure.Data;
using Bwadl.Accounting.Infrastructure.ExternalServices;
using Bwadl.Accounting.Infrastructure.Extensions;
using Bwadl.Accounting.Infrastructure.Messaging;
using Bwadl.Accounting.Infrastructure.Monitoring;
using Bwadl.Accounting.Infrastructure.Repositories;
using Bwadl.Accounting.Infrastructure.Security;
using Bwadl.Accounting.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Bwadl.Accounting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var logger = Log.ForContext(typeof(DependencyInjection));
        logger.Information("Registering Infrastructure services");

        // Configuration Services
        services.AddConfigurationServices(configuration);

        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                logger.Warning("No database connection string found, using in-memory database");
                options.UseInMemoryDatabase("BwadlAccountingDb");
            }
            else
            {
                logger.Information("Using PostgreSQL database");
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });
            }

            // Enable sensitive data logging in development
            if (configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Current User Service
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        
        // Domain Services
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddScoped<IPermissionService, PermissionService>();
        
        // External Services
        services.AddHttpClient<IEmailService, EmailService>();
        
        // Messaging
        services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
        
        // Caching - register memory cache first, then our cache service
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, RedisService>();
        
        // Security
        services.AddSingleton<ISecretManager, SecretManager>();

        // Monitoring and Health Checks
        services.AddMonitoring(configuration);

        logger.Information("Infrastructure services registered successfully");
        return services;
    }
}
