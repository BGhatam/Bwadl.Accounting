using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.ValueObjects;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    int Id,
    string? Email,
    string? MobileNumber,
    string? MobileCountryCode,
    string? IdentityId,
    string? IdentityType,
    string? NameEn,
    string? NameAr,
    string? Language
) : IRequest<UserDto>;
