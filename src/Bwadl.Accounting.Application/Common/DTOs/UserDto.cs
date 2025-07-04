using Bwadl.Accounting.Domain.ValueObjects;

namespace Bwadl.Accounting.Application.Common.DTOs;

public record UserDto(
    int Id,
    string? Email,
    string? MobileNumber,
    string? MobileCountryCode,
    string? IdentityId,
    string? IdentityType,
    string? NameEn,
    string? NameAr,
    string Language,
    bool IsEmailVerified,
    bool IsMobileVerified,
    bool IsUserVerified,
    DateTime? EmailVerifiedAt,
    DateTime? MobileVerifiedAt,
    DateTime? UserVerifiedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
