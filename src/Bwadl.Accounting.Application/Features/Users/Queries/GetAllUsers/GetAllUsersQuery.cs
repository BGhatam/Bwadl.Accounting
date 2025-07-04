using Bwadl.Accounting.Application.Common.DTOs;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;
