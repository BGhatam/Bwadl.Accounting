namespace Bwadl.Accounting.Domain.ValueObjects;

public class Mobile
{
    public string Number { get; private set; } = null!;
    public string CountryCode { get; private set; } = null!;

    private Mobile() { } // For EF Core

    public Mobile(string number, string countryCode = "+966")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        ArgumentException.ThrowIfNullOrWhiteSpace(countryCode);

        Number = number;
        CountryCode = countryCode;
    }

    public string FullNumber => $"{CountryCode}{Number}";

    public override string ToString() => FullNumber;
}
