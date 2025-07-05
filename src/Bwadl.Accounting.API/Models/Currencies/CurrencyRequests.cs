namespace Bwadl.Accounting.API.Models.Currencies;

public record CreateCurrencyRequest(
    string CurrencyCode,
    string CurrencyName,
    int DecimalPlaces = 2
);

public record UpdateCurrencyRequest(
    string CurrencyName,
    int DecimalPlaces
);
