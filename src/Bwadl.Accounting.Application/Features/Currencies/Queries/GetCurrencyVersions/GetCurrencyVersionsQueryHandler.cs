using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Currencies.Queries.GetCurrencyVersions;

public class GetCurrencyVersionsQueryHandler : IRequestHandler<GetCurrencyVersionsQuery, List<CurrencyDto>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<GetCurrencyVersionsQueryHandler> _logger;

    public GetCurrencyVersionsQueryHandler(
        ICurrencyRepository currencyRepository,
        ILogger<GetCurrencyVersionsQueryHandler> logger)
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task<List<CurrencyDto>> Handle(GetCurrencyVersionsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all versions for currency: {CurrencyCode}", request.CurrencyCode);

        var currencies = await _currencyRepository.GetAllVersionsAsync(request.CurrencyCode);
        
        return currencies.Select(c => c.ToDto()).ToList();
    }
}
