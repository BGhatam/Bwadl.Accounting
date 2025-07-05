using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Exceptions;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Currencies.Commands.UpdateCurrency;

public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, CurrencyDto>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<UpdateCurrencyCommandHandler> _logger;

    public UpdateCurrencyCommandHandler(
        ICurrencyRepository currencyRepository,
        ILogger<UpdateCurrencyCommandHandler> logger)
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task<CurrencyDto> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating currency: {CurrencyCode}", request.CurrencyCode);

        // Validate decimal places
        if (request.DecimalPlaces < 0 || request.DecimalPlaces > 8)
        {
            throw new InvalidDecimalPlacesException(request.DecimalPlaces);
        }

        // Update currency - this will throw CurrencyNotFoundException if not found
        var updatedCurrency = await _currencyRepository.UpdateAsync(
            request.CurrencyCode,
            request.CurrencyName,
            request.DecimalPlaces,
            request.UpdatedBy
        );

        _logger.LogInformation("Currency updated successfully: {CurrencyCode}", updatedCurrency.CurrencyCode);

        return updatedCurrency.ToDto();
    }
}
