using Bwadl.Accounting.Application.Features.Currencies.Commands.UpdateCurrency;
using FluentValidation;

namespace Bwadl.Accounting.Application.Validators;

public class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
{
    public UpdateCurrencyCommandValidator()
    {
        // Currency code validation (ISO 4217 standard)
        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .WithMessage("Currency code is required")
            .Length(3)
            .WithMessage("Currency code must be exactly 3 characters")
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Currency code must be 3 uppercase letters (e.g., USD, EUR, SAR)");

        // Currency name validation
        RuleFor(x => x.CurrencyName)
            .NotEmpty()
            .WithMessage("Currency name is required")
            .MinimumLength(2)
            .WithMessage("Currency name must be at least 2 characters")
            .MaximumLength(100)
            .WithMessage("Currency name must not exceed 100 characters");

        // Decimal places validation
        RuleFor(x => x.DecimalPlaces)
            .InclusiveBetween(0, 8)
            .WithMessage("Decimal places must be between 0 and 8");

        // Updated by validation
        RuleFor(x => x.UpdatedBy)
            .NotEmpty()
            .WithMessage("Updated by is required")
            .MaximumLength(255)
            .WithMessage("Updated by must not exceed 255 characters");
    }
}
