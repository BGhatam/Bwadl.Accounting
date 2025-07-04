namespace Bwadl.Accounting.Application.Features.Currencies.DTOs;

public record CreateCurrencyRequest(
    string CurrencyCode,
    string CurrencyName,
    int DecimalPlaces = 2
);

public record UpdateCurrencyRequest(
    string CurrencyName,
    int DecimalPlaces
);
