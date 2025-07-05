using Bwadl.Accounting.Application.Features.Currencies.Commands.DeleteCurrency;
using FluentValidation;

namespace Bwadl.Accounting.Application.Validators;

public class DeleteCurrencyCommandValidator : AbstractValidator<DeleteCurrencyCommand>
{
    public DeleteCurrencyCommandValidator()
    {
        // Currency code validation
        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .WithMessage("Currency code is required")
            .Length(3)
            .WithMessage("Currency code must be exactly 3 characters")
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Currency code must be 3 uppercase letters (e.g., USD, EUR, SAR)");
    }
}
