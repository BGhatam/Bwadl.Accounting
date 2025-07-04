using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Infrastructure.Data.Seed;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Apply any pending migrations
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");

            // Seed initial data if database is empty
            if (!await context.Currencies.AnyAsync())
            {
                await SeedCurrenciesAsync(context, logger);
            }

            // Seed system admin users
            if (!await context.Users.AnyAsync())
            {
                await SeedSystemAdminsAsync(serviceProvider, logger);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private static async Task SeedCurrenciesAsync(ApplicationDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding initial currency data");

        var currencies = new[]
        {
            new Currency("SAR", "Saudi Riyals", 2, "System"),
            new Currency("BHD", "Bahraini Dinars", 3, "System"),
            new Currency("USD", "US Dollar", 2, "System"),
            new Currency("EUR", "Euro", 2, "System"),
            new Currency("GBP", "British Pound Sterling", 2, "System"),
            new Currency("JPY", "Japanese Yen", 0, "System"),
            new Currency("CAD", "Canadian Dollar", 2, "System"),
            new Currency("AUD", "Australian Dollar", 2, "System"),
            new Currency("CHF", "Swiss Franc", 2, "System"),
            new Currency("CNY", "Chinese Yuan", 2, "System"),
            new Currency("SEK", "Swedish Krona", 2, "System"),
            new Currency("NZD", "New Zealand Dollar", 2, "System")
        };

        await context.Currencies.AddRangeAsync(currencies);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} currencies", currencies.Length);
    }

    private static async Task SeedSystemAdminsAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        
        logger.LogInformation("Seeding system admin users");
        
        try
        {
            var admins = await userRepository.CreateSystemAdminsAsync();
            logger.LogInformation("Successfully seeded {Count} system admin users", admins.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding system admin users");
            throw;
        }
    }
}
