using Bwadl.Accounting.Domain.Common;

namespace Bwadl.Accounting.Domain.Entities;

public class Currency : IVersionedEntity
{
    public int Id { get; private set; }                    // Surrogate key
    public string CurrencyCode { get; private set; } = null!;  // Natural key
    
    public string CurrencyName { get; private set; } = null!;
    public int DecimalPlaces { get; private set; } = 2;
    
    // IVersionedEntity implementation - public setters for EF Core/Infrastructure
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = null!;

    // Private constructor for EF Core
    private Currency() { }

    // Constructor for new currency
    public Currency(string currencyCode, string currencyName, int decimalPlaces, string createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        var now = DateTime.UtcNow;
        
        CurrencyCode = currencyCode.ToUpperInvariant();
        CurrencyName = currencyName;
        DecimalPlaces = decimalPlaces;
        Version = 1;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
        CreatedAt = now;
        UpdatedAt = now;
    }

    // Update method for business logic
    public void Update(string currencyName, int decimalPlaces, string updatedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(updatedBy);

        CurrencyName = currencyName;
        DecimalPlaces = decimalPlaces;
        UpdatedBy = updatedBy;
        // Version and UpdatedAt will be handled by DbContext
    }
}
