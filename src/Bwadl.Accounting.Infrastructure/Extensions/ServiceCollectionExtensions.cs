using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Bwadl.Accounting.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
