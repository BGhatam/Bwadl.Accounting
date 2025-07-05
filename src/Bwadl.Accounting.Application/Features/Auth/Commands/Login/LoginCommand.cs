using Bwadl.Accounting.Application.Common.DTOs;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Username,
    string Password,
    bool RememberMe = false
) : IRequest<AuthResponse>;
