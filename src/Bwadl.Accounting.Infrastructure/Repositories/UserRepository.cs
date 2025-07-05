using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Domain.ValueObjects;
using Bwadl.Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, IPasswordService passwordService, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.CreatedByUser)
            .Include(u => u.UpdatedByUser)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        
        return await _context.Users
            .Include(u => u.CreatedByUser)
            .Include(u => u.UpdatedByUser)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<User?> GetByMobileAsync(string mobile, string countryCode = "+966", CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mobile);
        ArgumentException.ThrowIfNullOrWhiteSpace(countryCode);
        
        return await _context.Users
            .Include(u => u.CreatedByUser)
            .Include(u => u.UpdatedByUser)
            .FirstOrDefaultAsync(u => u.Mobile!.Number == mobile && u.Mobile.CountryCode == countryCode, cancellationToken);
    }

    public async Task<User?> GetByIdentityAsync(string identityId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identityId);
        
        return await _context.Users
            .Include(u => u.CreatedByUser)
            .Include(u => u.UpdatedByUser)
            .FirstOrDefaultAsync(u => u.Identity!.Id == identityId, cancellationToken);
    }

    public async Task<User?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        
        return await _context.Users
            .Include(u => u.CreatedByUser)
            .Include(u => u.UpdatedByUser)
            .FirstOrDefaultAsync(u => u.SessionId == sessionId, cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
        
        return await _context.Users
            .Include(u => u.CreatedByUser)
            .Include(u => u.UpdatedByUser)
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && 
                                    u.RefreshTokenExpiry > DateTime.UtcNow, 
                                    cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.CreatedByUser)
            .Include(u => u.UpdatedByUser)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max page size

        return await _context.Users
            .Include(u => u.CreatedByUser)
            .Include(u => u.UpdatedByUser)
            .OrderBy(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        
        return await _context.Users
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<bool> ExistsByMobileAsync(string mobile, string countryCode = "+966", CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mobile);
        ArgumentException.ThrowIfNullOrWhiteSpace(countryCode);
        
        return await _context.Users
            .AnyAsync(u => u.Mobile!.Number == mobile && u.Mobile.CountryCode == countryCode, cancellationToken);
    }

    public async Task<bool> ExistsByIdentityAsync(string identityId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identityId);
        
        return await _context.Users
            .AnyAsync(u => u.Identity!.Id == identityId, cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePasswordAsync(int userId, string newPasswordHash, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);
        
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user != null)
        {
            user.SetPassword("", newPasswordHash); // Pass empty string for password since we already have the hash
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
        
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user != null)
        {
            // Set refresh token with default expiry based on JWT settings
            var expiry = DateTime.UtcNow.AddDays(30); // Default 30 days, should be configurable
            user.UpdateRefreshToken(refreshToken, expiry);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<User>> CreateSystemAdminsAsync(CancellationToken cancellationToken = default)
    {
        var admins = new List<User>();

        try
        {
            // Create built-in system admin first
            var builtIn = await GetByEmailAsync("bwadl@bwadl.sa", cancellationToken);
            if (builtIn == null)
            {
                builtIn = User.CreateSystemAdmin(
                    "bwadl@bwadl.sa",
                    "System Admin",
                    "مدير النظام",
                    new Mobile("0540869387", "+966"),
                    Identity.CreateNationalId("0123456789")
                );
                builtIn.SetPassword("@P@ssme1!", _passwordService.HashPassword("@P@ssme1!"));
                await _context.Users.AddAsync(builtIn, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Created built-in system admin user");
            }
            admins.Add(builtIn);

            // Create main system admin
            var sysAdmin = await GetByEmailAsync("balghatam@bwadl.sa", cancellationToken);
            if (sysAdmin == null)
            {
                sysAdmin = User.CreateSystemAdmin(
                    "balghatam@bwadl.sa",
                    "Baqer Alghatam",
                    "باقر الغتم",
                    new Mobile("0540869386", "+966"),
                    Identity.CreateNationalId("1234567890")
                );
                sysAdmin.SetPassword("@P@ssme1!", _passwordService.HashPassword("@P@ssme1!"));
                // Set created by using EF Core change tracking
                _context.Entry(sysAdmin).Property("CreatedByUserId").CurrentValue = builtIn.Id;
                await _context.Users.AddAsync(sysAdmin, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Created main system admin user");
            }
            admins.Add(sysAdmin);

            // Create other admin users
            var otherAdmins = new[]
            {
                new { Email = "amarei@bwatech.sa", Mobile = "0508358979", NameEn = "Alaa Marei", NameAr = "علاء مرعي", IdentityId = "2234567890", IdentityType = IdentityType.IQM, CountryCode = "+966" },
                new { Email = "afakhry@bwatech.sa", Mobile = "0520869391", NameEn = "Ahmed Fakhry", NameAr = "أحمد فخري", IdentityId = "2234567891", IdentityType = IdentityType.IQM, CountryCode = "+966" },
                new { Email = "aalaali@bwadl.sa", Mobile = "37770244", NameEn = "Ali Alaali", NameAr = "علي العالي", IdentityId = "860204816", IdentityType = IdentityType.GCCID, CountryCode = "+973" },
                new { Email = "halqarooni@bwadl.sa", Mobile = "33355188", NameEn = "Hashem Alqarooni", NameAr = "هاشم القاروني", IdentityId = "860204817", IdentityType = IdentityType.GCCID, CountryCode = "+973" },
                new { Email = "amar@arzagplus.com", Mobile = "0562288227", NameEn = "Amar Alameer", NameAr = "عمار الأمير", IdentityId = "1860204814", IdentityType = IdentityType.NID, CountryCode = "+966" },
                new { Email = "aalqassab@bwadl.sa", Mobile = "32122208", NameEn = "Ali AlQassab", NameAr = "علي القصاب", IdentityId = "1000204814", IdentityType = IdentityType.NID, CountryCode = "+973" },
                new { Email = "aalsaffar@bwadl.sa", Mobile = "33776930", NameEn = "Ali Alsaffar", NameAr = "علي الصفار", IdentityId = "1030204814", IdentityType = IdentityType.NID, CountryCode = "+973" },
                new { Email = "malaali@bwadl.sa", Mobile = "36267636", NameEn = "Mahdi Alaali", NameAr = "مهدي العالي", IdentityId = "1030204815", IdentityType = IdentityType.NID, CountryCode = "+973" }
            };

            foreach (var adminData in otherAdmins)
            {
                var admin = await GetByEmailAsync(adminData.Email, cancellationToken);
                if (admin == null)
                {
                    var identity = adminData.IdentityType switch
                    {
                        IdentityType.NID => Identity.CreateNationalId(adminData.IdentityId),
                        IdentityType.GCCID => Identity.CreateGccId(adminData.IdentityId),
                        IdentityType.IQM => Identity.CreateIqama(adminData.IdentityId),
                        _ => Identity.CreateNationalId(adminData.IdentityId)
                    };

                    admin = User.CreateSystemAdmin(
                        adminData.Email,
                        adminData.NameEn,
                        adminData.NameAr,
                        new Mobile(adminData.Mobile, adminData.CountryCode),
                        identity
                    );
                    admin.SetPassword("@P@ssme1!", _passwordService.HashPassword("@P@ssme1!"));
                    // Set created by using EF Core change tracking
                    _context.Entry(admin).Property("CreatedByUserId").CurrentValue = sysAdmin.Id;
                    await _context.Users.AddAsync(admin, cancellationToken);
                    _logger.LogInformation("Created admin user: {Email}", adminData.Email);
                }
                admins.Add(admin);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully created {Count} admin users", admins.Count);

            return admins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating system admin users");
            throw;
        }
    }
}
