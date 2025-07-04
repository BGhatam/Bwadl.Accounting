using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bwadl.Accounting.Infrastructure.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly ApplicationDbContext _context;

    public CurrencyRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Currency?> GetCurrentVersionAsync(string currencyCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        
        return await _context.Currencies
            .Where(c => c.CurrencyCode == currencyCode.ToUpperInvariant())
            .OrderByDescending(c => c.Version)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Currency>> GetAllVersionsAsync(string currencyCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        
        return await _context.Currencies
            .Where(c => c.CurrencyCode == currencyCode.ToUpperInvariant())
            .OrderByDescending(c => c.Version)
            .ToListAsync();
    }

    public async Task<List<Currency>> GetAllCurrentVersionsAsync()
    {
        // Get the latest version of each currency
        var latestVersions = await _context.Currencies
            .GroupBy(c => c.CurrencyCode)
            .Select(g => new { CurrencyCode = g.Key, MaxVersion = g.Max(c => c.Version) })
            .ToListAsync();

        var currencies = new List<Currency>();
        foreach (var item in latestVersions)
        {
            var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.CurrencyCode == item.CurrencyCode && c.Version == item.MaxVersion);
            if (currency != null)
            {
                currencies.Add(currency);
            }
        }

        return currencies.OrderBy(c => c.CurrencyCode).ToList();
    }

    public async Task<Currency> CreateAsync(Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);
        
        // Check if currency code already exists
        var exists = await ExistsAsync(currency.CurrencyCode);
        if (exists)
        {
            throw new InvalidOperationException($"Currency with code '{currency.CurrencyCode}' already exists.");
        }

        await _context.Currencies.AddAsync(currency);
        await _context.SaveChangesAsync();
        return currency;
    }

    public async Task<Currency> UpdateAsync(string currencyCode, string currencyName, int decimalPlaces, string updatedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(updatedBy);

        var currentCurrency = await GetCurrentVersionAsync(currencyCode);
        if (currentCurrency == null)
        {
            throw new InvalidOperationException($"Currency with code '{currencyCode}' not found.");
        }

        // Create new currency version by creating a new entity
        var newCurrency = new Currency(currencyCode, currencyName, decimalPlaces, updatedBy);
        
        await _context.Currencies.AddAsync(newCurrency);
        await _context.SaveChangesAsync();
        return newCurrency;
    }

    public async Task<bool> ExistsAsync(string currencyCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        
        return await _context.Currencies
            .AnyAsync(c => c.CurrencyCode == currencyCode.ToUpperInvariant());
    }
}
