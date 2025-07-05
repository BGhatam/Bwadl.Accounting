using Bwadl.Accounting.Application.Common.DTOs;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Currencies.Commands.UpdateCurrency;

public record UpdateCurrencyCommand(
    string CurrencyCode,
    string CurrencyName,
    int DecimalPlaces,
    string UpdatedBy
) : IRequest<CurrencyDto>;
