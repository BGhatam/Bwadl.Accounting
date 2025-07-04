using Bwadl.Accounting.Application.Features.Currencies.DTOs;
using FluentValidation;

namespace Bwadl.Accounting.Application.Features.Currencies.Validators;

public class CreateCurrencyRequestValidator : AbstractValidator<CreateCurrencyRequest>
{
    public CreateCurrencyRequestValidator()
    {
        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .WithMessage("Currency code is required")
            .Length(3)
            .WithMessage("Currency code must be exactly 3 characters")
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency code must be 3 uppercase letters");

        RuleFor(x => x.CurrencyName)
            .NotEmpty()
            .WithMessage("Currency name is required")
            .MaximumLength(100)
            .WithMessage("Currency name cannot exceed 100 characters");

        RuleFor(x => x.DecimalPlaces)
            .InclusiveBetween(0, 8)
            .WithMessage("Decimal places must be between 0 and 8");
    }
}

public class UpdateCurrencyRequestValidator : AbstractValidator<UpdateCurrencyRequest>
{
    public UpdateCurrencyRequestValidator()
    {
        RuleFor(x => x.CurrencyName)
            .NotEmpty()
            .WithMessage("Currency name is required")
            .MaximumLength(100)
            .WithMessage("Currency name cannot exceed 100 characters");

        RuleFor(x => x.DecimalPlaces)
            .InclusiveBetween(0, 8)
            .WithMessage("Decimal places must be between 0 and 8");
    }
}
