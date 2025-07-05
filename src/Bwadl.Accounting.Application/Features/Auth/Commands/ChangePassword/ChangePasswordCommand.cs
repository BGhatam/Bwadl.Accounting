using MediatR;

namespace Bwadl.Accounting.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(
    int UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : IRequest<bool>;
