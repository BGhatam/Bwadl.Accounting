using Bwadl.Accounting.Domain.Entities;

namespace Bwadl.Accounting.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<IEnumerable<Role>> GetActiveRolesAsync();
    Task<Role> CreateAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
}

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(int id);
    Task<Permission?> GetByNameAsync(string name);
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<IEnumerable<Permission>> GetActivePermissionsAsync();
    Task<Permission> CreateAsync(Permission permission);
    Task<Permission> UpdateAsync(Permission permission);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
}

public interface IUserRoleRepository
{
    Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> GetActiveUserRolesAsync(int userId);
    Task<UserRole> AssignRoleAsync(UserRole userRole);
    Task RemoveRoleAsync(int userId, int roleId);
    Task<bool> HasRoleAsync(int userId, string roleName);
    Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId);
}

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByIdAsync(int id);
    Task<ApiKey?> GetByKeyHashAsync(string keyHash);
    Task<IEnumerable<ApiKey>> GetAllAsync();
    Task<IEnumerable<ApiKey>> GetActiveApiKeysAsync();
    Task<IEnumerable<ApiKey>> GetUserApiKeysAsync(int userId);
    Task<ApiKey> CreateAsync(ApiKey apiKey);
    Task<ApiKey> UpdateAsync(ApiKey apiKey);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

public interface IPermissionService
{
    Task<IEnumerable<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId);
    Task<bool> HasPermissionAsync(int userId, string permission);
    Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName);
}
