using Bwadl.Accounting.Application.Common.Interfaces;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Infrastructure.Caching;
using Bwadl.Accounting.Infrastructure.Configuration;
using Bwadl.Accounting.Infrastructure.Data.Repositories;
using Bwadl.Accounting.Infrastructure.ExternalServices;
using Bwadl.Accounting.Infrastructure.Extensions;
using Bwadl.Accounting.Infrastructure.Messaging;
using Bwadl.Accounting.Infrastructure.Security;
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

        // Repositories
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        
        // External Services
        services.AddHttpClient<IEmailService, EnhancedEmailService>();
        
        // Messaging
        services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
        
        // Caching - register memory cache first, then our cache service
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, RedisService>();
        
        // Security
        services.AddSingleton<ISecretManager, SecretManager>();

        logger.Information("Infrastructure services registered successfully");
        return services;
    }
}
