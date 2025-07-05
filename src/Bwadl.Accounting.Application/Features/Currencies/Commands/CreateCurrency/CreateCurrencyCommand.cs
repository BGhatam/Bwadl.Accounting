using Bwadl.Accounting.Application.Common.DTOs;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Currencies.Commands.CreateCurrency;

public record CreateCurrencyCommand(
    string CurrencyCode,
    string CurrencyName,
    int DecimalPlaces,
    string CreatedBy
) : IRequest<CurrencyDto>;
