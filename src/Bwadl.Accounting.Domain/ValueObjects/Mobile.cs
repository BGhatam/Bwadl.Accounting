using System.Linq;

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

    public static bool TryParse(string? value, out Mobile? mobile)
    {
        mobile = null;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            // Remove spaces and common separators
            var cleanValue = value.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            
            // Simple parsing logic - can be enhanced
            if (cleanValue.StartsWith("+"))
            {
                // Extract country code and number
                var countryCodeEnd = 4; // Assuming country code is 3-4 digits
                if (cleanValue.Length > countryCodeEnd)
                {
                    var countryCode = cleanValue.Substring(0, countryCodeEnd);
                    var number = cleanValue.Substring(countryCodeEnd);
                    mobile = new Mobile(number, countryCode);
                    return true;
                }
            }
            else if (cleanValue.All(char.IsDigit) && cleanValue.Length >= 9)
            {
                // Assume Saudi number without country code
                mobile = new Mobile(cleanValue, "+966");
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}