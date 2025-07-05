using MediatR;

namespace Bwadl.Accounting.Application.Features.Currencies.Commands.DeleteCurrency;

public record DeleteCurrencyCommand(
    string CurrencyCode
) : IRequest<bool>;
