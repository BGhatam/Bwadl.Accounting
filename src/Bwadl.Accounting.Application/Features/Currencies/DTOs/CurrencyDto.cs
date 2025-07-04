using Bwadl.Accounting.Domain.Entities;

namespace Bwadl.Accounting.Application.Features.Currencies.DTOs;

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
