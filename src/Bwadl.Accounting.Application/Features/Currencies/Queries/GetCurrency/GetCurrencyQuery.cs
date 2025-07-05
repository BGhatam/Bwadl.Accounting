using Bwadl.Accounting.Application.Common.DTOs;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Currencies.Queries.GetCurrency;

public record GetCurrencyQuery(
    string CurrencyCode
) : IRequest<CurrencyDto?>;
