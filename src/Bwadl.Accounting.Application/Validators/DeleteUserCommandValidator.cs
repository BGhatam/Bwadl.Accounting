using Bwadl.Accounting.Application.Features.Users.Commands.DeleteUser;
using FluentValidation;

namespace Bwadl.Accounting.Application.Validators;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        // User ID validation
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}
