namespace Bwadl.Accounting.Domain.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() 
        : base("Invalid username or password.")
    {
    }

    public InvalidCredentialsException(string message) 
        : base(message)
    {
    }
}

public class AccountLockedException : Exception
{
    public DateTime LockedUntil { get; }

    public AccountLockedException(DateTime lockedUntil) 
        : base($"Account is locked until {lockedUntil:yyyy-MM-dd HH:mm:ss} UTC.")
    {
        LockedUntil = lockedUntil;
    }
}

public class InvalidTokenException : Exception
{
    public InvalidTokenException() 
        : base("Invalid or expired token.")
    {
    }

    public InvalidTokenException(string message) 
        : base(message)
    {
    }
}

public class TokenExpiredException : Exception
{
    public DateTime ExpiredAt { get; }

    public TokenExpiredException(DateTime expiredAt) 
        : base($"Token expired at {expiredAt:yyyy-MM-dd HH:mm:ss} UTC.")
    {
        ExpiredAt = expiredAt;
    }
}

public class UnauthorizedAccessException : Exception
{
    public string? RequiredPermission { get; }

    public UnauthorizedAccessException() 
        : base("Access denied.")
    {
    }

    public UnauthorizedAccessException(string requiredPermission) 
        : base($"Access denied. Required permission: '{requiredPermission}'.")
    {
        RequiredPermission = requiredPermission;
    }
}

public class RegistrationFailedException : Exception
{
    public RegistrationFailedException(string reason) 
        : base($"Registration failed: {reason}")
    {
    }
}

public class WeakPasswordException : Exception
{
    public WeakPasswordException() 
        : base("Password does not meet security requirements.")
    {
    }

    public WeakPasswordException(string requirements) 
        : base($"Password does not meet security requirements: {requirements}")
    {
    }
}

public class RefreshTokenExpiredException : Exception
{
    public RefreshTokenExpiredException() 
        : base("Refresh token has expired.")
    {
    }
}

public class InvalidRefreshTokenException : Exception
{
    public InvalidRefreshTokenException() 
        : base("Invalid refresh token.")
    {
    }
}
