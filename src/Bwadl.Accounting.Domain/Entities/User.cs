using Bwadl.Accounting.Domain.Common;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Bwadl.Accounting.Domain.Entities;

public class User : IVersionedEntity
{
    private static readonly Regex EmailRegex = new(@".+\@.+\..+", RegexOptions.Compiled);
    private static readonly Regex PasswordRegex = new(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?])[A-Za-z\d!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]{8,}$", RegexOptions.Compiled);

    public int Id { get; private set; }

    // Core identity fields
    public string? Email { get; private set; }
    public Mobile? Mobile { get; private set; }
    public Identity? Identity { get; private set; }

    // Session and device
    public string? SessionId { get; private set; }
    public string? DeviceToken { get; private set; }

    // Authentication
    public string? PasswordHash { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiry { get; private set; }

    // Account locking
    public bool IsLocked { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public int FailedLoginAttempts { get; private set; }

    // Names (multilingual)
    public string? NameEn { get; private set; }
    public string? NameAr { get; private set; }
    public Language Language { get; private set; } = Language.English;

    // Verification status
    public bool IsEmailVerified { get; private set; }
    public bool IsMobileVerified { get; private set; }
    public bool IsUserVerified { get; private set; }

    // Verification timestamps
    public DateTime? EmailVerifiedAt { get; private set; }
    public DateTime? MobileVerifiedAt { get; private set; }
    public DateTime? UserVerifiedAt { get; private set; }

    // References
    public int? LastChosenParticipantId { get; private set; }
    public int? CreatedByUserId { get; private set; }
    public int? UpdatedByUserId { get; private set; }

    // Additional data
    public string? Details { get; private set; } // JSON string for flexible data

    // Navigation properties
    public User? CreatedByUser { get; private set; }
    public User? UpdatedByUser { get; private set; }
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<ApiKey> ApiKeys { get; private set; } = new List<ApiKey>();

    // IVersionedEntity implementation
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = null!;

    // Private constructor for EF Core
    private User() { }

    // Public constructor
    public User(string? email = null, Mobile? mobile = null, Identity? identity = null, 
                string? nameEn = null, string? nameAr = null, Language language = Language.English)
    {
        // At least one of email, mobile, or identity must be provided
        if (string.IsNullOrWhiteSpace(email) && mobile == null && identity == null)
        {
            throw new ArgumentException("At least one of email, mobile, or identity must be provided");
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            ValidateEmail(email);
            Email = email;
        }

        Mobile = mobile;
        Identity = identity;
        NameEn = nameEn;
        NameAr = nameAr;
        Language = language;

        IsEmailVerified = false;
        IsMobileVerified = false;
        IsUserVerified = false;
    }

    // Password methods
    public void SetPassword(string password, string hashedPassword)
    {
        ValidatePassword(password);
        PasswordHash = hashedPassword;
    }

    public bool HasPassword() => !string.IsNullOrEmpty(PasswordHash);

    public bool ComparePassword(string candidatePassword, IPasswordService passwordService)
    {
        if (string.IsNullOrEmpty(PasswordHash))
        {
            return false;
        }
        
        return passwordService.VerifyPassword(candidatePassword, PasswordHash);
    }

    // Update methods
    public void UpdateEmail(string email)
    {
        ValidateEmail(email);
        Email = email;
        IsEmailVerified = false;
        EmailVerifiedAt = null;
    }

    public void UpdateMobile(Mobile mobile)
    {
        Mobile = mobile;
        IsMobileVerified = false;
        MobileVerifiedAt = null;
    }

    public void UpdateIdentity(Identity identity)
    {
        Identity = identity;
        IsUserVerified = false;
        UserVerifiedAt = null;
    }

    public void UpdateNames(string? nameEn, string? nameAr)
    {
        NameEn = nameEn;
        NameAr = nameAr;
    }

    public void UpdateLanguage(Language language)
    {
        Language = language;
    }

    public void UpdateSessionId(string? sessionId)
    {
        SessionId = sessionId;
    }

    public void UpdateDeviceToken(string? deviceToken)
    {
        DeviceToken = deviceToken;
    }

    public void UpdateDetails(string? details)
    {
        Details = details;
    }

    // Refresh token methods
    public void UpdateRefreshToken(string refreshToken, DateTime expiry)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiry = expiry;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiry = null;
    }

    public bool IsRefreshTokenValid()
    {
        return !string.IsNullOrEmpty(RefreshToken) && 
               RefreshTokenExpiry.HasValue && 
               RefreshTokenExpiry > DateTime.UtcNow;
    }

    // Account locking methods
    public void LockAccount(DateTime? until = null)
    {
        IsLocked = true;
        LockedUntil = until ?? DateTime.UtcNow.AddMinutes(30);
    }

    public void UnlockAccount()
    {
        IsLocked = false;
        LockedUntil = null;
        FailedLoginAttempts = 0;
    }

    public void IncrementFailedLoginAttempts()
    {
        FailedLoginAttempts++;
        
        // Lock account after 5 failed attempts
        if (FailedLoginAttempts >= 5)
        {
            LockAccount();
        }
    }

    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttempts = 0;
    }

    // Verification methods
    public void VerifyEmail()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            throw new InvalidOperationException("Cannot verify email - no email address set");
        }

        IsEmailVerified = true;
        EmailVerifiedAt = DateTime.UtcNow;
    }

    public void VerifyMobile()
    {
        if (Mobile == null)
        {
            throw new InvalidOperationException("Cannot verify mobile - no mobile number set");
        }

        IsMobileVerified = true;
        MobileVerifiedAt = DateTime.UtcNow;
    }

    public void VerifyUser()
    {
        if (string.IsNullOrWhiteSpace(Details))
        {
            throw new InvalidOperationException("User cannot be verified until details are populated from Nafath");
        }

        IsUserVerified = true;
        UserVerifiedAt = DateTime.UtcNow;
    }

    public void UnverifyEmail()
    {
        IsEmailVerified = false;
        EmailVerifiedAt = null;
    }

    public void UnverifyMobile()
    {
        IsMobileVerified = false;
        MobileVerifiedAt = null;
    }

    public void UnverifyUser()
    {
        IsUserVerified = false;
        UserVerifiedAt = null;
    }

    // Helper methods
    public string GetDisplayName()
    {
        return Language == Language.Arabic && !string.IsNullOrWhiteSpace(NameAr) 
            ? NameAr 
            : NameEn ?? Email ?? Mobile?.FullNumber ?? "Unknown User";
    }

    public bool IsFullyVerified()
    {
        return IsEmailVerified && IsMobileVerified && IsUserVerified;
    }

    // Validation methods
    private static void ValidateEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        
        if (!EmailRegex.IsMatch(email))
        {
            throw new ArgumentException("Please fill a valid email address");
        }
    }

    private static void ValidatePassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        
        if (!PasswordRegex.IsMatch(password))
        {
            throw new ArgumentException("Password must be at least 8 characters long, include at least one number and one special character");
        }
    }

    // Create system admin users (static factory method)
    public static User CreateSystemAdmin(string email, string nameEn, string nameAr, Mobile mobile, Identity identity)
    {
        var user = new User(email, mobile, identity, nameEn, nameAr, Language.English);
        user.IsEmailVerified = true;
        user.IsMobileVerified = true;
        user.IsUserVerified = true;
        user.EmailVerifiedAt = DateTime.UtcNow;
        user.MobileVerifiedAt = DateTime.UtcNow;
        user.UserVerifiedAt = DateTime.UtcNow;
        return user;
    }
}