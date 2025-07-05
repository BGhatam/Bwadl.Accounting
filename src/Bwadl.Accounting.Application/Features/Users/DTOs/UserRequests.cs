using System.ComponentModel.DataAnnotations;

namespace Bwadl.Accounting.Application.Features.Users.DTOs;

public record CreateUserRequest(
    [EmailAddress][StringLength(255)]
    string? Email,
    
    [StringLength(20)]
    string? MobileNumber,
    
    [StringLength(10)]
    string? MobileCountryCode,
    
    [StringLength(50)]
    string? IdentityId,
    
    [StringLength(10)]
    string? IdentityType,
    
    [StringLength(255)]
    string? NameEn,
    
    [StringLength(255)]
    string? NameAr,
    
    [StringLength(2)]
    string? Language,
    
    [StringLength(100)]
    string? Password
);

public record UpdateUserRequest(
    [EmailAddress][StringLength(255)]
    string? Email,
    
    [StringLength(20)]
    string? MobileNumber,
    
    [StringLength(10)]
    string? MobileCountryCode,
    
    [StringLength(50)]
    string? IdentityId,
    
    [StringLength(10)]
    string? IdentityType,
    
    [StringLength(255)]
    string? NameEn,
    
    [StringLength(255)]
    string? NameAr,
    
    [StringLength(2)]
    string? Language
);
