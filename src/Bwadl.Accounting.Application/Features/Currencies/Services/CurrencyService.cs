using Bwadl.Accounting.Application.Features.Currencies.DTOs;
using Bwadl.Accounting.Application.Features.Currencies.Interfaces;
using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Interfaces;

namespace Bwadl.Accounting.Application.Features.Currencies.Services;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;

    public CurrencyService(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository ?? throw new ArgumentNullException(nameof(currencyRepository));
    }

    public async Task<CurrencyDto?> GetCurrentVersionAsync(string currencyCode)
    {
        var currency = await _currencyRepository.GetCurrentVersionAsync(currencyCode);
        return currency?.ToDto();
    }

    public async Task<List<CurrencyDto>> GetAllVersionsAsync(string currencyCode)
    {
        var currencies = await _currencyRepository.GetAllVersionsAsync(currencyCode);
        return currencies.Select(c => c.ToDto()).ToList();
    }

    public async Task<List<CurrencyDto>> GetAllCurrentVersionsAsync()
    {
        var currencies = await _currencyRepository.GetAllCurrentVersionsAsync();
        return currencies.Select(c => c.ToDto()).ToList();
    }

    public async Task<CurrencyDto> CreateAsync(CreateCurrencyRequest request, string createdBy)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        var currency = new Currency(request.CurrencyCode, request.CurrencyName, request.DecimalPlaces, createdBy);
        var createdCurrency = await _currencyRepository.CreateAsync(currency);
        return createdCurrency.ToDto();
    }

    public async Task<CurrencyDto> UpdateAsync(string currencyCode, UpdateCurrencyRequest request, string updatedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(updatedBy);

        var updatedCurrency = await _currencyRepository.UpdateAsync(currencyCode, request.CurrencyName, request.DecimalPlaces, updatedBy);
        return updatedCurrency.ToDto();
    }

    public async Task<bool> ExistsAsync(string currencyCode)
    {
        return await _currencyRepository.ExistsAsync(currencyCode);
    }
}

// Extension method to convert Currency to CurrencyDto
public static class CurrencyExtensions
{
    public static CurrencyDto ToDto(this Currency currency)
    {
        return new CurrencyDto(
            currency.Id,
            currency.CurrencyCode,
            currency.CurrencyName,
            currency.DecimalPlaces,
            currency.Version,
            currency.CreatedAt,
            currency.CreatedBy,
            currency.UpdatedAt,
            currency.UpdatedBy
        );
    }
}
