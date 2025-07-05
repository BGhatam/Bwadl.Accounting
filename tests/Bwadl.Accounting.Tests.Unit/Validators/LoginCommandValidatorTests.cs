using Bwadl.Accounting.Application.Features.Auth.Commands.Login;
using Bwadl.Accounting.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Bwadl.Accounting.Tests.Unit.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Empty()
    {
        // Arrange
        var command = new LoginCommand("", "password123");

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Username)
              .WithErrorMessage("Username is required");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        // Arrange
        var command = new LoginCommand("user@example.com", "");

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Too_Long()
    {
        // Arrange
        var longUsername = new string('a', 256); // 256 characters
        var command = new LoginCommand(longUsername, "password123");

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Username)
              .WithErrorMessage("Username must not exceed 255 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new LoginCommand("user@example.com", "password123");

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
