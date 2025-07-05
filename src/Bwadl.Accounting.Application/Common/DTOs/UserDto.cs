using Bwadl.Accounting.Domain.ValueObjects;
using Bwadl.Accounting.Domain.Entities;

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

// Extension method to convert User entity to UserDto
public static class UserExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.Email,
            user.Mobile?.Number,
            user.Mobile?.CountryCode,
            user.Identity?.Id,
            user.Identity?.Type.ToString().ToLowerInvariant(),
            user.NameEn,
            user.NameAr,
            user.Language.ToCode(),
            user.IsEmailVerified,
            user.IsMobileVerified,
            user.IsUserVerified,
            user.EmailVerifiedAt,
            user.MobileVerifiedAt,
            user.UserVerifiedAt,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}
