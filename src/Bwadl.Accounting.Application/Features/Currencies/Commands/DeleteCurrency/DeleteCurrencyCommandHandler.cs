using Bwadl.Accounting.Domain.Exceptions;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Currencies.Commands.DeleteCurrency;

public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, bool>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<DeleteCurrencyCommandHandler> _logger;

    public DeleteCurrencyCommandHandler(
        ICurrencyRepository currencyRepository,
        ILogger<DeleteCurrencyCommandHandler> logger)
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting currency: {CurrencyCode}", request.CurrencyCode);

        // Check if currency exists
        var currency = await _currencyRepository.GetCurrentVersionAsync(request.CurrencyCode);
        if (currency == null)
        {
            throw new CurrencyNotFoundException(request.CurrencyCode);
        }

        // TODO: Add business logic to check if currency is in use
        // This would require checking if any transactions, accounts, or other entities use this currency
        // For now, we'll assume the repository handles this validation

        try
        {
            await _currencyRepository.DeleteAsync(request.CurrencyCode);
            
            _logger.LogInformation("Currency deleted successfully: {CurrencyCode}", request.CurrencyCode);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Currency deletion failed - currency in use: {CurrencyCode}", request.CurrencyCode);
            throw new CurrencyInUseException(request.CurrencyCode, ex.Message);
        }
    }
}
