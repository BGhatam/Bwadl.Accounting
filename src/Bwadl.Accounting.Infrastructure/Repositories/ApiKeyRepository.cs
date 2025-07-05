using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bwadl.Accounting.Infrastructure.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly ApplicationDbContext _context;

    public ApiKeyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiKey?> GetByIdAsync(int id)
    {
        return await _context.ApiKeys
            .Include(ak => ak.User)
            .FirstOrDefaultAsync(ak => ak.Id == id);
    }

    public async Task<ApiKey?> GetByKeyHashAsync(string keyHash)
    {
        return await _context.ApiKeys
            .Include(ak => ak.User)
            .FirstOrDefaultAsync(ak => ak.KeyHash == keyHash);
    }

    public async Task<IEnumerable<ApiKey>> GetAllAsync()
    {
        return await _context.ApiKeys
            .Include(ak => ak.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApiKey>> GetActiveApiKeysAsync()
    {
        return await _context.ApiKeys
            .Include(ak => ak.User)
            .Where(ak => ak.IsValidKey)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApiKey>> GetUserApiKeysAsync(int userId)
    {
        return await _context.ApiKeys
            .Include(ak => ak.User)
            .Where(ak => ak.UserId == userId)
            .ToListAsync();
    }

    public async Task<ApiKey> CreateAsync(ApiKey apiKey)
    {
        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();
        return apiKey;
    }

    public async Task<ApiKey> UpdateAsync(ApiKey apiKey)
    {
        _context.ApiKeys.Update(apiKey);
        await _context.SaveChangesAsync();
        return apiKey;
    }

    public async Task DeleteAsync(int id)
    {
        var apiKey = await _context.ApiKeys.FindAsync(id);
        if (apiKey != null)
        {
            _context.ApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.ApiKeys.AnyAsync(ak => ak.Id == id);
    }
}
