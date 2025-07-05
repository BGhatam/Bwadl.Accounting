using Bwadl.Accounting.Application.Common.DTOs;

namespace Bwadl.Accounting.API.Models.Users;

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
    public UserDto User { get; set; } = null!;
    public UserMetadata Metadata { get; set; } = null!;
}

public class UserMetadata
{
    public double ProfileCompleteness { get; set; }
    public int LastLoginDays { get; set; }
    public TimeSpan AccountAge { get; set; }
}
