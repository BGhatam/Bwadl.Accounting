using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Currencies.Queries.GetAllCurrencies;

public class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, List<CurrencyDto>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<GetAllCurrenciesQueryHandler> _logger;

    public GetAllCurrenciesQueryHandler(
        ICurrencyRepository currencyRepository,
        ILogger<GetAllCurrenciesQueryHandler> logger)
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task<List<CurrencyDto>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all current versions of currencies");

        var currencies = await _currencyRepository.GetAllCurrentVersionsAsync();
        
        return currencies.Select(c => c.ToDto()).ToList();
    }
}
