using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Exceptions;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtService jwtService,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        // Validate passwords match
        if (request.Password != request.ConfirmPassword)
        {
            throw new RegistrationFailedException("Password and confirmation password do not match");
        }

        // Check if user with email already exists
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", request.Email);
                throw new DuplicateEmailException(request.Email);
            }
        }

        // Validate password strength
        if (!IsPasswordStrong(request.Password))
        {
            throw new WeakPasswordException("Password must be at least 8 characters long and contain uppercase, lowercase, numbers, and special characters");
        }

        // Create mobile if provided
        Mobile? mobile = null;
        if (!string.IsNullOrWhiteSpace(request.Mobile))
        {
            mobile = new Mobile(request.Mobile, "+966"); // Default country code
        }

        // Hash password
        var passwordHash = _passwordService.HashPassword(request.Password);

        // Create user
        var user = new User(
            email: request.Email,
            mobile: mobile,
            identity: null,
            nameEn: request.NameEn,
            nameAr: request.NameAr,
            language: Language.English
        );

        // Set password
        user.SetPassword(request.Password, passwordHash);

        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(createdUser, new List<string>(), new List<string>());
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Update user's refresh token
        await _userRepository.UpdateRefreshTokenAsync(createdUser.Id, refreshToken, cancellationToken);

        _logger.LogInformation("Registration successful for user: {UserId}", createdUser.Id);

        return new AuthResponse
        {
            AccessToken = accessToken.Token,
            RefreshToken = refreshToken,
            ExpiresAt = accessToken.ExpiresAt,
            User = createdUser.ToUserDto(),
            Roles = new List<string>(),
            Permissions = new List<string>()
        };
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
