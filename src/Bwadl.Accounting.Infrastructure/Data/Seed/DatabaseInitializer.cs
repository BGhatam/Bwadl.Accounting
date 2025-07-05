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

            // Seed roles and permissions
            if (!await context.Roles.AnyAsync())
            {
                await SeedRolesAndPermissionsAsync(context, logger);
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
            new Currency("USD", "United States Dollar", 2, "System"),
            new Currency("EUR", "Euro", 2, "System"),
            new Currency("GBP", "British Pound Sterling", 2, "System"),
            new Currency("AED", "UAE Dirham", 2, "System"),
            new Currency("KWD", "Kuwaiti Dinar", 3, "System"),
            new Currency("BHD", "Bahraini Dinar", 3, "System"),
            new Currency("QAR", "Qatari Riyal", 2, "System"),
            new Currency("OMR", "Omani Rial", 3, "System"),
            new Currency("JOD", "Jordanian Dinar", 3, "System")
        };

        await context.Currencies.AddRangeAsync(currencies);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} currencies", currencies.Length);
    }

    private static async Task SeedRolesAndPermissionsAsync(ApplicationDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding initial roles and permissions");

        // Create permissions first
        var permissions = new[]
        {
            new Permission(Permission.SystemPermissions.UsersRead, "Users", "Read", "Read user information"),
            new Permission(Permission.SystemPermissions.UsersWrite, "Users", "Write", "Create and update users"),
            new Permission(Permission.SystemPermissions.UsersDelete, "Users", "Delete", "Delete users"),
            new Permission(Permission.SystemPermissions.AdminRead, "Admin", "Read", "Read admin functions"),
            new Permission(Permission.SystemPermissions.AdminWrite, "Admin", "Write", "Admin write operations"),
            new Permission(Permission.SystemPermissions.AdminDelete, "Admin", "Delete", "Admin delete operations"),
            new Permission(Permission.SystemPermissions.SystemRead, "System", "Read", "System read access"),
            new Permission(Permission.SystemPermissions.SystemWrite, "System", "Write", "System write access"),
            new Permission(Permission.SystemPermissions.SystemDelete, "System", "Delete", "System delete access"),
            new Permission(Permission.SystemPermissions.ApiKeysRead, "ApiKeys", "Read", "Read API keys"),
            new Permission(Permission.SystemPermissions.ApiKeysWrite, "ApiKeys", "Write", "Create and update API keys"),
            new Permission(Permission.SystemPermissions.ApiKeysDelete, "ApiKeys", "Delete", "Delete API keys")
        };

        // Set versioned entity properties for permissions
        foreach (var permission in permissions)
        {
            permission.CreatedAt = DateTime.UtcNow;
            permission.CreatedBy = "System";
            permission.UpdatedAt = DateTime.UtcNow;
            permission.UpdatedBy = "System";
            permission.Version = 1;
        }

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();

        // Create roles
        var roles = new[]
        {
            new Role(Role.SystemRoles.SuperAdmin, "Super Administrator with all permissions"),
            new Role(Role.SystemRoles.Admin, "Administrator role with full permissions"),
            new Role(Role.SystemRoles.User, "Regular user role with limited permissions"),
            new Role(Role.SystemRoles.Guest, "Guest role with read-only access")
        };

        // Set versioned entity properties for roles
        foreach (var role in roles)
        {
            role.CreatedAt = DateTime.UtcNow;
            role.CreatedBy = "System";
            role.UpdatedAt = DateTime.UtcNow;
            role.UpdatedBy = "System";
            role.Version = 1;
        }

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        // Assign permissions to roles
        var superAdminRole = roles.First(r => r.Name == Role.SystemRoles.SuperAdmin);
        var adminRole = roles.First(r => r.Name == Role.SystemRoles.Admin);
        var userRole = roles.First(r => r.Name == Role.SystemRoles.User);
        var guestRole = roles.First(r => r.Name == Role.SystemRoles.Guest);

        var rolePermissions = new List<RolePermission>();

        // SuperAdmin gets all permissions
        foreach (var permission in permissions)
        {
            var rolePermission = new RolePermission(superAdminRole.Id, permission.Id);
            rolePermission.CreatedAt = DateTime.UtcNow;
            rolePermission.CreatedBy = "System";
            rolePermission.UpdatedAt = DateTime.UtcNow;
            rolePermission.UpdatedBy = "System";
            rolePermission.Version = 1;
            rolePermissions.Add(rolePermission);
        }

        // Admin gets user management and admin permissions
        var adminPermissionNames = new[]
        {
            Permission.SystemPermissions.UsersRead,
            Permission.SystemPermissions.UsersWrite,
            Permission.SystemPermissions.UsersDelete,
            Permission.SystemPermissions.AdminRead,
            Permission.SystemPermissions.AdminWrite,
            Permission.SystemPermissions.ApiKeysRead,
            Permission.SystemPermissions.ApiKeysWrite
        };

        foreach (var permissionName in adminPermissionNames)
        {
            var permission = permissions.First(p => p.Name == permissionName);
            var rolePermission = new RolePermission(adminRole.Id, permission.Id);
            rolePermission.CreatedAt = DateTime.UtcNow;
            rolePermission.CreatedBy = "System";
            rolePermission.UpdatedAt = DateTime.UtcNow;
            rolePermission.UpdatedBy = "System";
            rolePermission.Version = 1;
            rolePermissions.Add(rolePermission);
        }

        // User gets basic permissions
        var userPermissionNames = new[]
        {
            Permission.SystemPermissions.UsersRead
        };

        foreach (var permissionName in userPermissionNames)
        {
            var permission = permissions.First(p => p.Name == permissionName);
            var rolePermission = new RolePermission(userRole.Id, permission.Id);
            rolePermission.CreatedAt = DateTime.UtcNow;
            rolePermission.CreatedBy = "System";
            rolePermission.UpdatedAt = DateTime.UtcNow;
            rolePermission.UpdatedBy = "System";
            rolePermission.Version = 1;
            rolePermissions.Add(rolePermission);
        }

        // Guest gets only read permissions
        var guestPermissionNames = new[]
        {
            Permission.SystemPermissions.UsersRead
        };

        foreach (var permissionName in guestPermissionNames)
        {
            var permission = permissions.First(p => p.Name == permissionName);
            var rolePermission = new RolePermission(guestRole.Id, permission.Id);
            rolePermission.CreatedAt = DateTime.UtcNow;
            rolePermission.CreatedBy = "System";
            rolePermission.UpdatedAt = DateTime.UtcNow;
            rolePermission.UpdatedBy = "System";
            rolePermission.Version = 1;
            rolePermissions.Add(rolePermission);
        }

        await context.RolePermissions.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {RoleCount} roles, {PermissionCount} permissions, and {RolePermissionCount} role permissions", 
            roles.Length, permissions.Length, rolePermissions.Count);
    }

    private static async Task SeedSystemAdminsAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var userRoleRepository = scope.ServiceProvider.GetRequiredService<IUserRoleRepository>();
        var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

        logger.LogInformation("Seeding system admin users");

        // Create super admin user
        var superAdminUser = new User("admin@bwadl.com", null, null, "System Administrator", "مدير النظام");
        var hashedPassword = passwordService.HashPassword("Admin@123!");
        superAdminUser.SetPassword("Admin@123!", hashedPassword);

        // Set versioned entity properties
        superAdminUser.CreatedAt = DateTime.UtcNow;
        superAdminUser.CreatedBy = "System";
        superAdminUser.UpdatedAt = DateTime.UtcNow;
        superAdminUser.UpdatedBy = "System";
        superAdminUser.Version = 1;

        await userRepository.CreateAsync(superAdminUser);

        // Assign SuperAdmin role
        var superAdminRole = await roleRepository.GetByNameAsync(Role.SystemRoles.SuperAdmin);
        if (superAdminRole != null)
        {
            var userRole = new UserRole(superAdminUser.Id, superAdminRole.Id);
            userRole.CreatedAt = DateTime.UtcNow;
            userRole.CreatedBy = "System";
            userRole.UpdatedAt = DateTime.UtcNow;
            userRole.UpdatedBy = "System";
            userRole.Version = 1;

            await userRoleRepository.AssignRoleAsync(userRole);
        }

        logger.LogInformation("Successfully seeded system admin user: {Email}", superAdminUser.Email);
    }
}
