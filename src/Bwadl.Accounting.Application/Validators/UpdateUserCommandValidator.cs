using Bwadl.Accounting.Application.Features.Users.Commands.UpdateUser;
using FluentValidation;

namespace Bwadl.Accounting.Application.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        // Email validation (when provided)
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(255)
            .WithMessage("Email must not exceed 255 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        // Mobile validation (when provided)
        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .WithMessage("Mobile number is required when mobile country code is provided")
            .When(x => !string.IsNullOrWhiteSpace(x.MobileCountryCode));

        RuleFor(x => x.MobileCountryCode)
            .NotEmpty()
            .WithMessage("Mobile country code is required when mobile number is provided")
            .When(x => !string.IsNullOrWhiteSpace(x.MobileNumber));

        // Identity validation (when provided)
        RuleFor(x => x.IdentityType)
            .NotEmpty()
            .WithMessage("Identity type is required when identity ID is provided")
            .When(x => !string.IsNullOrWhiteSpace(x.IdentityId));

        RuleFor(x => x.IdentityId)
            .NotEmpty()
            .WithMessage("Identity ID is required when identity type is provided")
            .When(x => !string.IsNullOrWhiteSpace(x.IdentityType));

        // Names validation
        RuleFor(x => x.NameEn)
            .MaximumLength(255)
            .WithMessage("English name must not exceed 255 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.NameEn));

        RuleFor(x => x.NameAr)
            .MaximumLength(255)
            .WithMessage("Arabic name must not exceed 255 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));

        // Language validation
        RuleFor(x => x.Language)
            .Must(lang => string.IsNullOrWhiteSpace(lang) || lang.ToLower() == "en" || lang.ToLower() == "ar")
            .WithMessage("Language must be 'en' or 'ar'")
            .When(x => !string.IsNullOrWhiteSpace(x.Language));
    }
}
