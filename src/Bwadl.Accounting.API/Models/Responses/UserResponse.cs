using Bwadl.Accounting.Application.Common.DTOs;

namespace Bwadl.Accounting.API.Models.Responses;

/// <summary>
/// DEPRECATED: This class has been replaced with UserDto from Application layer.
/// Use Bwadl.Accounting.Application.Common.DTOs.UserDto instead.
/// This class will be removed in a future version.
/// </summary>
[Obsolete("Use Bwadl.Accounting.Application.Common.DTOs.UserDto instead. This class will be removed in a future version.")]
public record UserResponse(
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

/// <summary>
/// DEPRECATED: This class has been moved to Application layer.
/// Use Bwadl.Accounting.Application.Features.Users.DTOs.PagedResponse instead.
/// This class will be removed in a future version.
/// </summary>
[Obsolete("Use Bwadl.Accounting.Application.Features.Users.DTOs.PagedResponse instead. This class will be removed in a future version.")]
public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
/// DEPRECATED: This class has been moved to Application layer.
/// Use Bwadl.Accounting.Application.Features.Users.DTOs.UserDetailResponse instead.
/// This class will be removed in a future version.
/// </summary>
[Obsolete("Use Bwadl.Accounting.Application.Features.Users.DTOs.UserDetailResponse instead. This class will be removed in a future version.")]
public class UserDetailResponse
{
    public UserResponse User { get; set; } = null!;
    public UserMetadata Metadata { get; set; } = null!;
}

/// <summary>
/// DEPRECATED: This class has been moved to Application layer.
/// Use Bwadl.Accounting.Application.Features.Users.DTOs.UserMetadata instead.
/// This class will be removed in a future version.
/// </summary>
[Obsolete("Use Bwadl.Accounting.Application.Features.Users.DTOs.UserMetadata instead. This class will be removed in a future version.")]
public class UserMetadata
{
    public double ProfileCompleteness { get; set; }
    public int LastLoginDays { get; set; }
    public TimeSpan AccountAge { get; set; }
}
