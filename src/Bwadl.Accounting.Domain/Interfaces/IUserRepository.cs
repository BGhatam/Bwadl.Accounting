using Bwadl.Accounting.Domain.Entities;

namespace Bwadl.Accounting.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByMobileAsync(string mobile, string countryCode = "+966", CancellationToken cancellationToken = default);
    Task<User?> GetByIdentityAsync(string identityId, CancellationToken cancellationToken = default);
    Task<User?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByMobileAsync(string mobile, string countryCode = "+966", CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdentityAsync(string identityId, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<List<User>> CreateSystemAdminsAsync(CancellationToken cancellationToken = default);
}
