using Bwadl.Accounting.Application.Features.Auth.Commands.Register;
using FluentValidation;

namespace Bwadl.Accounting.Application.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        // At least one identifier must be provided
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Mobile))
            .WithMessage("Either Email or Mobile number must be provided");

        // Email validation (when provided)
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(255)
            .WithMessage("Email must not exceed 255 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        // Mobile validation (when provided)
        RuleFor(x => x.Mobile)
            .NotEmpty()
            .WithMessage("Mobile number cannot be empty when provided")
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Mobile number must be in valid international format")
            .When(x => !string.IsNullOrWhiteSpace(x.Mobile));

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?])[A-Za-z\d!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]{8,}$")
            .WithMessage("Password must include at least one letter, one number, and one special character");

        // Confirm password validation
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Password confirmation is required")
            .Equal(x => x.Password)
            .WithMessage("Password and confirmation password do not match");

        // Names validation
        RuleFor(x => x.NameEn)
            .MaximumLength(255)
            .WithMessage("English name must not exceed 255 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.NameEn));

        RuleFor(x => x.NameAr)
            .MaximumLength(255)
            .WithMessage("Arabic name must not exceed 255 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));

        // At least one name should be provided
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.NameEn) || !string.IsNullOrWhiteSpace(x.NameAr))
            .WithMessage("At least one name (English or Arabic) must be provided");
    }
}
