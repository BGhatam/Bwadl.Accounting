using Bwadl.Accounting.Application.Common.DTOs;

namespace Bwadl.Accounting.API.Models.Responses;

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

// V2 API Response Models
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

public class UserDetailResponse
{
    public UserResponse User { get; set; } = null!;
    public UserMetadata Metadata { get; set; } = null!;
}

public class UserMetadata
{
    public double ProfileCompleteness { get; set; }
    public int LastLoginDays { get; set; }
    public TimeSpan AccountAge { get; set; }
}
