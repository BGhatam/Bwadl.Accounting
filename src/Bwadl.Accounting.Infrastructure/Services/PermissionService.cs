using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bwadl.Accounting.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _context;

    public PermissionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var permissions = await _context.UserRoles
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(ur => ur.UserId == userId && 
                        ur.IsActive && 
                        (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow) && 
                        ur.Role.IsActive)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Where(rp => rp.IsActive && rp.Permission.IsActive)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        return permissions;
    }

    public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId)
    {
        var permissions = await _context.UserRoles
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(ur => ur.UserId == userId && 
                        ur.IsActive && 
                        (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow) && 
                        ur.Role.IsActive)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Where(rp => rp.IsActive && rp.Permission.IsActive)
            .Select(rp => rp.Permission)
            .Distinct()
            .ToListAsync();

        return permissions;
    }

    public async Task<bool> HasPermissionAsync(int userId, string permission)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .AnyAsync(ur => ur.UserId == userId && 
                           ur.IsActive && 
                           (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow) && 
                           ur.Role.IsActive &&
                           ur.Role.RolePermissions.Any(rp => rp.IsActive && 
                                                           rp.Permission.IsActive && 
                                                           rp.Permission.Name == permission));
    }

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName)
    {
        var permissions = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(r => r.Name == roleName && r.IsActive)
            .SelectMany(r => r.RolePermissions)
            .Where(rp => rp.IsActive && rp.Permission.IsActive)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        return permissions;
    }
}
