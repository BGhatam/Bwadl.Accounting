namespace Bwadl.Accounting.Domain.ValueObjects;

public class Identity
{
    public string Id { get; private set; } = null!;
    public IdentityType Type { get; private set; }
    public string? ExpiryDate { get; private set; }
    public DateType? DateType { get; private set; }

    private Identity() { } // For EF Core

    public Identity(string id, IdentityType type, string? expiryDate = null, DateType? dateType = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        
        Id = id;
        Type = type;
        ExpiryDate = expiryDate;
        DateType = dateType;
    }

    public static Identity CreateNationalId(string id, string? expiryDate = null, DateType? dateType = null)
        => new(id, IdentityType.NID, expiryDate, dateType);

    public static Identity CreateGccId(string id, string? expiryDate = null, DateType? dateType = null)
        => new(id, IdentityType.GCCID, expiryDate, dateType);

    public static Identity CreateIqama(string id, string? expiryDate = null, DateType? dateType = null)
        => new(id, IdentityType.IQM, expiryDate, dateType);
}

public enum IdentityType
{
    NID,    // National ID
    GCCID,  // GCC ID
    IQM     // Iqama
}

public enum DateType
{
    Hijri,
    Gregorian
}
