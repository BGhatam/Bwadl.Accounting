using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.ValueObjects;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Name,
    string Email,
    UserType Type
) : IRequest<UserDto>;
