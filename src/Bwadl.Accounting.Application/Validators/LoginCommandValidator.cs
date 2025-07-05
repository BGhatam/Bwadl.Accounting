using Bwadl.Accounting.Application.Features.Auth.Commands.Login;
using FluentValidation;

namespace Bwadl.Accounting.Application.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        // Username validation
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MaximumLength(255)
            .WithMessage("Username must not exceed 255 characters");

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(1)
            .WithMessage("Password cannot be empty");
    }
}
