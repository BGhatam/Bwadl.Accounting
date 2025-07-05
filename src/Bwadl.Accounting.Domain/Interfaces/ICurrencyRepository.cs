using Bwadl.Accounting.Domain.Entities;

namespace Bwadl.Accounting.Domain.Interfaces;

public interface ICurrencyRepository
{
    Task<Currency?> GetCurrentVersionAsync(string currencyCode);
    Task<List<Currency>> GetAllVersionsAsync(string currencyCode);
    Task<List<Currency>> GetAllCurrentVersionsAsync();
    Task<Currency> CreateAsync(Currency currency);
    Task<Currency> UpdateAsync(string currencyCode, string currencyName, int decimalPlaces, string updatedBy);
    Task DeleteAsync(string currencyCode);
    Task<bool> ExistsAsync(string currencyCode);
}
