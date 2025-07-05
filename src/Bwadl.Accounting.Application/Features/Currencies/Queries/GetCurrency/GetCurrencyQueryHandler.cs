using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Currencies.Queries.GetCurrency;

public class GetCurrencyQueryHandler : IRequestHandler<GetCurrencyQuery, CurrencyDto?>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<GetCurrencyQueryHandler> _logger;

    public GetCurrencyQueryHandler(
        ICurrencyRepository currencyRepository,
        ILogger<GetCurrencyQueryHandler> logger)
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task<CurrencyDto?> Handle(GetCurrencyQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting currency: {CurrencyCode}", request.CurrencyCode);

        var currency = await _currencyRepository.GetCurrentVersionAsync(request.CurrencyCode);
        
        return currency?.ToDto();
    }
}
