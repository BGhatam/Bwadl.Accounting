namespace Bwadl.Accounting.Domain.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(int userId) 
        : base($"User with ID '{userId}' was not found.")
    {
    }

    public UserNotFoundException(string identifier) 
        : base($"User with identifier '{identifier}' was not found.")
    {
    }
}

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email) 
        : base($"A user with email '{email}' already exists.")
    {
    }
}
