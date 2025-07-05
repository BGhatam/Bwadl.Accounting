using Bwadl.Accounting.Application.Common.DTOs;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Currencies.Queries.GetCurrencyVersions;

public record GetCurrencyVersionsQuery(
    string CurrencyCode
) : IRequest<List<CurrencyDto>>;
