using Bwadl.Accounting.Domain.Entities;

namespace Bwadl.Accounting.Application.Common.DTOs;

public record CurrencyDto(
    int Id,
    string CurrencyCode,
    string CurrencyName,
    int DecimalPlaces,
    int Version,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime UpdatedAt,
    string UpdatedBy
);

// Extension method to convert Currency to CurrencyDto
public static class CurrencyExtensions
{
    public static CurrencyDto ToDto(this Currency currency)
    {
        return new CurrencyDto(
            currency.Id,
            currency.CurrencyCode,
            currency.CurrencyName,
            currency.DecimalPlaces,
            currency.Version,
            currency.CreatedAt,
            currency.CreatedBy,
            currency.UpdatedAt,
            currency.UpdatedBy
        );
    }
}
