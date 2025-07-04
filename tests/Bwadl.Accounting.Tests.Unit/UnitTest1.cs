using AutoFixture;
using AutoFixture.Xunit2;
using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Exceptions;
using Bwadl.Accounting.Domain.ValueObjects;
using FluentAssertions;

namespace Bwadl.Accounting.Tests.Unit;

public class UserEntityTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void User_Constructor_With_Email_Should_Create_Valid_User()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var user = new User(email: email);

        // Assert
        user.Email.Should().Be(email);
        user.Language.Should().Be(Language.English);
        user.IsEmailVerified.Should().BeFalse();
        user.IsMobileVerified.Should().BeFalse();
        user.IsUserVerified.Should().BeFalse();
    }

    [Fact]
    public void User_Constructor_With_Mobile_Should_Create_Valid_User()
    {
        // Arrange
        var mobile = new Mobile("0501234567", "+966");

        // Act
        var user = new User(mobile: mobile);

        // Assert
        user.Mobile.Should().NotBeNull();
        user.Mobile!.Number.Should().Be("0501234567");
        user.Mobile.CountryCode.Should().Be("+966");
        user.Language.Should().Be(Language.English);
    }

    [Fact]
    public void User_Constructor_With_Identity_Should_Create_Valid_User()
    {
        // Arrange
        var identity = new Identity("1234567890", IdentityType.NID, "2025-12-31", DateType.Hijri);

        // Act
        var user = new User(identity: identity);

        // Assert
        user.Identity.Should().NotBeNull();
        user.Identity!.Id.Should().Be("1234567890");
        user.Identity.Type.Should().Be(IdentityType.NID);
        user.Language.Should().Be(Language.English);
    }

    [Fact]
    public void User_Constructor_Without_Identifiers_Should_Throw_Exception()
    {
        // Act & Assert
        var action = () => new User();
        action.Should().Throw<ArgumentException>()
            .WithMessage("At least one of email, mobile, or identity must be provided");
    }

    [Fact]
    public void UpdateEmail_Should_Update_Email()
    {
        // Arrange
        var user = new User(email: "original@example.com");
        var newEmail = "updated@example.com";

        // Act
        user.UpdateEmail(newEmail);

        // Assert
        user.Email.Should().Be(newEmail);
    }

    [Fact]
    public void UpdateMobile_Should_Update_Mobile()
    {
        // Arrange
        var user = new User(email: "test@example.com");
        var mobile = new Mobile("0501234567", "+966");

        // Act
        user.UpdateMobile(mobile);

        // Assert
        user.Mobile.Should().NotBeNull();
        user.Mobile!.Number.Should().Be("0501234567");
        user.Mobile.CountryCode.Should().Be("+966");
    }

    [Fact]
    public void UpdateIdentity_Should_Update_Identity()
    {
        // Arrange
        var user = new User(email: "test@example.com");
        var identity = new Identity("1234567890", IdentityType.NID, "2025-12-31", DateType.Hijri);

        // Act
        user.UpdateIdentity(identity);

        // Assert
        user.Identity.Should().NotBeNull();
        user.Identity!.Id.Should().Be("1234567890");
        user.Identity.Type.Should().Be(IdentityType.NID);
        user.Identity.ExpiryDate.Should().Be("2025-12-31");
        user.Identity.DateType.Should().Be(DateType.Hijri);
    }

    [Fact]
    public void UpdateNames_Should_Update_Names()
    {
        // Arrange
        var user = new User(email: "test@example.com");
        var nameEn = "John Doe";
        var nameAr = "جون دو";

        // Act
        user.UpdateNames(nameEn, nameAr);

        // Assert
        user.NameEn.Should().Be(nameEn);
        user.NameAr.Should().Be(nameAr);
    }

    [Fact]
    public void VerifyEmail_Should_Set_Verification_Status()
    {
        // Arrange
        var user = new User(email: "test@example.com");

        // Act
        user.VerifyEmail();

        // Assert
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateLanguage_Should_Update_Language()
    {
        // Arrange
        var user = new User(email: "test@example.com");

        // Act
        user.UpdateLanguage(Language.Arabic);

        // Assert
        user.Language.Should().Be(Language.Arabic);
    }

    [Fact]
    public void UpdateDetails_Should_Store_Json_Data()
    {
        // Arrange
        var user = new User(email: "test@example.com");
        var details = "{\"preference\": \"dark_mode\", \"notifications\": true}";

        // Act
        user.UpdateDetails(details);

        // Assert
        user.Details.Should().Be(details);
    }

    [Fact]
    public void UpdateSessionId_Should_Update_SessionId()
    {
        // Arrange
        var user = new User(email: "test@example.com");
        var sessionId = "session_123456";

        // Act
        user.UpdateSessionId(sessionId);

        // Assert
        user.SessionId.Should().Be(sessionId);
    }

    [Fact]
    public void UpdateDeviceToken_Should_Update_DeviceToken()
    {
        // Arrange
        var user = new User(email: "test@example.com");
        var deviceToken = "device_token_123";

        // Act
        user.UpdateDeviceToken(deviceToken);

        // Assert
        user.DeviceToken.Should().Be(deviceToken);
    }
}