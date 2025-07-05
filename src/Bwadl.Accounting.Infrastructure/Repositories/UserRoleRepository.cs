using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bwadl.Accounting.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly ApplicationDbContext _context;

    public UserRoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetActiveUserRolesAsync(int userId)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId && 
                        ur.IsActive && 
                        (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow) && 
                        ur.Role.IsActive)
            .ToListAsync();
    }

    public async Task<UserRole> AssignRoleAsync(UserRole userRole)
    {
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();
        return userRole;
    }

    public async Task RemoveRoleAsync(int userId, int roleId)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        
        if (userRole != null)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasRoleAsync(int userId, string roleName)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.UserId == userId && 
                           ur.Role.Name == roleName && 
                           ur.IsActive && 
                           (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow) && 
                           ur.Role.IsActive);
    }

    public async Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId && 
                        ur.IsActive && 
                        (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow) && 
                        ur.Role.IsActive)
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    }
}
