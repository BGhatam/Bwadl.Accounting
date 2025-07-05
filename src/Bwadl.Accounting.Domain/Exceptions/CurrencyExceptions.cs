namespace Bwadl.Accounting.Domain.Exceptions;

public class CurrencyNotFoundException : Exception
{
    public CurrencyNotFoundException(string currencyCode) 
        : base($"Currency with code '{currencyCode}' was not found.")
    {
    }

    public CurrencyNotFoundException(int currencyId) 
        : base($"Currency with ID '{currencyId}' was not found.")
    {
    }
}

public class DuplicateCurrencyCodeException : Exception
{
    public DuplicateCurrencyCodeException(string currencyCode) 
        : base($"A currency with code '{currencyCode}' already exists.")
    {
    }
}

public class InvalidCurrencyCodeException : Exception
{
    public InvalidCurrencyCodeException(string currencyCode) 
        : base($"Invalid currency code '{currencyCode}'. Currency codes must be 3 characters long and contain only letters.")
    {
    }
}

public class InvalidDecimalPlacesException : Exception
{
    public InvalidDecimalPlacesException(int decimalPlaces) 
        : base($"Invalid decimal places '{decimalPlaces}'. Decimal places must be between 0 and 8.")
    {
    }
}

public class CurrencyInUseException : Exception
{
    public CurrencyInUseException(string currencyCode) 
        : base($"Currency '{currencyCode}' cannot be deleted because it is currently in use.")
    {
    }

    public CurrencyInUseException(string currencyCode, string usageContext) 
        : base($"Currency '{currencyCode}' cannot be deleted because it is currently in use in {usageContext}.")
    {
    }
}

public class BaseCurrencyException : Exception
{
    public BaseCurrencyException(string currencyCode) 
        : base($"Currency '{currencyCode}' is the base currency and cannot be deleted or deactivated.")
    {
    }
}

public class CurrencyConversionException : Exception
{
    public CurrencyConversionException(string fromCurrency, string toCurrency) 
        : base($"Cannot convert from '{fromCurrency}' to '{toCurrency}'. Exchange rate not available.")
    {
    }

    public CurrencyConversionException(string message) 
        : base(message)
    {
    }
}

public class ExchangeRateNotFoundException : Exception
{
    public ExchangeRateNotFoundException(string fromCurrency, string toCurrency) 
        : base($"Exchange rate from '{fromCurrency}' to '{toCurrency}' was not found.")
    {
    }

    public ExchangeRateNotFoundException(string fromCurrency, string toCurrency, DateTime date) 
        : base($"Exchange rate from '{fromCurrency}' to '{toCurrency}' for date '{date:yyyy-MM-dd}' was not found.")
    {
    }
}
