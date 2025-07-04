using Bwadl.Accounting.Application.Features.Currencies.DTOs;

namespace Bwadl.Accounting.Application.Features.Currencies.Interfaces;

public interface ICurrencyService
{
    Task<CurrencyDto?> GetCurrentVersionAsync(string currencyCode);
    Task<List<CurrencyDto>> GetAllVersionsAsync(string currencyCode);
    Task<List<CurrencyDto>> GetAllCurrentVersionsAsync();
    Task<CurrencyDto> CreateAsync(CreateCurrencyRequest request, string createdBy);
    Task<CurrencyDto> UpdateAsync(string currencyCode, UpdateCurrencyRequest request, string updatedBy);
    Task<bool> ExistsAsync(string currencyCode);
}
