using Bwadl.Accounting.Domain.Exceptions;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password change request for user: {UserId}", request.UserId);

        // Validate passwords match
        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new RegistrationFailedException("New password and confirmation password do not match");
        }

        // Get user
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Password change failed - user not found: {UserId}", request.UserId);
            throw new UserNotFoundException(request.UserId);
        }

        // Verify current password
        if (string.IsNullOrEmpty(user.PasswordHash) || !_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            _logger.LogWarning("Password change failed - invalid current password for user: {UserId}", request.UserId);
            throw new InvalidCredentialsException("Current password is incorrect");
        }

        // Validate new password strength
        if (!IsPasswordStrong(request.NewPassword))
        {
            throw new WeakPasswordException("New password must be at least 8 characters long and contain uppercase, lowercase, numbers, and special characters");
        }

        // Hash new password
        var newPasswordHash = _passwordService.HashPassword(request.NewPassword);

        // Update password
        await _userRepository.UpdatePasswordAsync(request.UserId, newPasswordHash, cancellationToken);

        _logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);

        return true;
    }

    private static bool IsPasswordStrong(string password)
    {
        if (password.Length < 8) return false;
        
        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
