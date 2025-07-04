using Bwadl.Accounting.Domain.Entities;
using Xunit;

namespace Bwadl.Accounting.Tests.Unit.Domain.Entities;

public class CurrencyTests
{
    [Fact]
    public void Constructor_ValidParameters_CreatesValidCurrency()
    {
        // Arrange
        var currencyCode = "USD";
        var currencyName = "US Dollar";
        var decimalPlaces = 2;
        var createdBy = "TestUser";

        // Act
        var currency = new Currency(currencyCode, currencyName, decimalPlaces, createdBy);

        // Assert
        Assert.Equal("USD", currency.CurrencyCode);
        Assert.Equal("US Dollar", currency.CurrencyName);
        Assert.Equal(2, currency.DecimalPlaces);
        Assert.Equal(1, currency.Version);
        Assert.Equal("TestUser", currency.CreatedBy);
        Assert.Equal("TestUser", currency.UpdatedBy);
        Assert.True(currency.CreatedAt <= DateTime.UtcNow);
        Assert.True(currency.UpdatedAt <= DateTime.UtcNow);
        Assert.Equal(currency.CreatedAt, currency.UpdatedAt);
    }

    [Fact]
    public void Constructor_LowercaseCurrencyCode_ConvertsToUppercase()
    {
        // Arrange & Act
        var currency = new Currency("usd", "US Dollar", 2, "TestUser");

        // Assert
        Assert.Equal("USD", currency.CurrencyCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyOrWhiteSpaceCurrencyCode_ThrowsArgumentException(string invalidCode)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Currency(invalidCode, "Test Currency", 2, "TestUser"));
    }

    [Fact]
    public void Constructor_NullCurrencyCode_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Currency(null!, "Test Currency", 2, "TestUser"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyOrWhiteSpaceCurrencyName_ThrowsArgumentException(string invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Currency("USD", invalidName, 2, "TestUser"));
    }

    [Fact]
    public void Constructor_NullCurrencyName_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Currency("USD", null!, 2, "TestUser"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyOrWhiteSpaceCreatedBy_ThrowsArgumentException(string invalidCreatedBy)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Currency("USD", "US Dollar", 2, invalidCreatedBy));
    }

    [Fact]
    public void Constructor_NullCreatedBy_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Currency("USD", "US Dollar", 2, null!));
    }

    [Fact]
    public void Update_ValidParameters_UpdatesPropertiesCorrectly()
    {
        // Arrange
        var currency = new Currency("USD", "US Dollar", 2, "TestUser");
        var originalCreatedAt = currency.CreatedAt;
        var originalCreatedBy = currency.CreatedBy;

        // Act
        currency.Update("United States Dollar", 4, "UpdatedUser");

        // Assert
        Assert.Equal("United States Dollar", currency.CurrencyName);
        Assert.Equal(4, currency.DecimalPlaces);
        Assert.Equal("UpdatedUser", currency.UpdatedBy);
        
        // Ensure audit fields are preserved correctly
        Assert.Equal(originalCreatedAt, currency.CreatedAt);
        Assert.Equal(originalCreatedBy, currency.CreatedBy);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_EmptyOrWhiteSpaceCurrencyName_ThrowsArgumentException(string invalidName)
    {
        // Arrange
        var currency = new Currency("USD", "US Dollar", 2, "TestUser");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            currency.Update(invalidName, 2, "UpdatedUser"));
    }

    [Fact]
    public void Update_NullCurrencyName_ThrowsArgumentNullException()
    {
        // Arrange
        var currency = new Currency("USD", "US Dollar", 2, "TestUser");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            currency.Update(null!, 2, "UpdatedUser"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_EmptyOrWhiteSpaceUpdatedBy_ThrowsArgumentException(string invalidUpdatedBy)
    {
        // Arrange
        var currency = new Currency("USD", "US Dollar", 2, "TestUser");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            currency.Update("Updated Name", 2, invalidUpdatedBy));
    }

    [Fact]
    public void Update_NullUpdatedBy_ThrowsArgumentNullException()
    {
        // Arrange
        var currency = new Currency("USD", "US Dollar", 2, "TestUser");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            currency.Update("Updated Name", 2, null!));
    }
}
