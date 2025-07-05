using Bwadl.Accounting.Application.Features.Auth.Commands.RefreshToken;
using FluentValidation;

namespace Bwadl.Accounting.Application.Validators;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        // Refresh token validation
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required")
            .MinimumLength(10)
            .WithMessage("Refresh token appears to be invalid")
            .MaximumLength(500)
            .WithMessage("Refresh token is too long");
    }
}
