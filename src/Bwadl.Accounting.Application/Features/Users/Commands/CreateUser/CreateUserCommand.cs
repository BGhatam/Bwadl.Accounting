using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.ValueObjects;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string? Email,
    string? MobileNumber,
    string? MobileCountryCode,
    string? IdentityId,
    string? IdentityType,
    string? NameEn,
    string? NameAr,
    string? Language,
    string? Password
) : IRequest<UserDto>;
