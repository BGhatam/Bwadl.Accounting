using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Exceptions;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Currencies.Commands.CreateCurrency;

public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, CurrencyDto>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<CreateCurrencyCommandHandler> _logger;

    public CreateCurrencyCommandHandler(
        ICurrencyRepository currencyRepository,
        ILogger<CreateCurrencyCommandHandler> logger)
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task<CurrencyDto> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating currency with code: {CurrencyCode}", request.CurrencyCode);

        // Validate currency code format
        if (string.IsNullOrWhiteSpace(request.CurrencyCode) || 
            request.CurrencyCode.Length != 3 || 
            !request.CurrencyCode.All(char.IsLetter))
        {
            throw new InvalidCurrencyCodeException(request.CurrencyCode);
        }

        // Validate decimal places
        if (request.DecimalPlaces < 0 || request.DecimalPlaces > 8)
        {
            throw new InvalidDecimalPlacesException(request.DecimalPlaces);
        }

        // Check if currency already exists
        var existingCurrency = await _currencyRepository.GetCurrentVersionAsync(request.CurrencyCode);
        if (existingCurrency != null)
        {
            _logger.LogWarning("Currency creation failed - currency already exists: {CurrencyCode}", request.CurrencyCode);
            throw new DuplicateCurrencyCodeException(request.CurrencyCode);
        }

        // Create currency
        var currency = new Currency(
            request.CurrencyCode.ToUpperInvariant(),
            request.CurrencyName,
            request.DecimalPlaces,
            request.CreatedBy
        );

        var createdCurrency = await _currencyRepository.CreateAsync(currency);

        _logger.LogInformation("Currency created successfully: {CurrencyCode}", createdCurrency.CurrencyCode);

        return createdCurrency.ToDto();
    }
}
