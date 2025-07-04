using Bwadl.Accounting.Application.Common.DTOs;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Users.Queries.GetUser;

public record GetUserQuery(int Id) : IRequest<UserDto?>;
