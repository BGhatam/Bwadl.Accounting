using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.ValueObjects;

namespace Bwadl.Accounting.Application.Common.Extensions;

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
