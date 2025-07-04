using Bwadl.Accounting.Domain.ValueObjects;

namespace Bwadl.Accounting.Application.Common.DTOs;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    UserType Type,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
