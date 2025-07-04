using MediatR;

namespace Bwadl.Accounting.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(int Id) : IRequest;
