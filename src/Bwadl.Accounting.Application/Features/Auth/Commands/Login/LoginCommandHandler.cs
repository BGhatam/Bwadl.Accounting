using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Exceptions;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtService jwtService,
        IUserRoleRepository userRoleRepository,
        IPermissionService permissionService,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _userRoleRepository = userRoleRepository;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for username: {Username}", request.Username);

        // Find user by email or mobile
        User? user = null;
        if (request.Username.Contains("@"))
        {
            user = await _userRepository.GetByEmailAsync(request.Username, cancellationToken);
        }
        else
        {
            user = await _userRepository.GetByMobileAsync(request.Username, "+966", cancellationToken);
        }

        if (user == null)
        {
            _logger.LogWarning("Login failed - user not found: {Username}", request.Username);
            throw new InvalidCredentialsException();
        }

        // Verify password
        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash ?? string.Empty))
        {
            _logger.LogWarning("Login failed - invalid password for user: {UserId}", user.Id);
            user.IncrementFailedLoginAttempts();
            await _userRepository.UpdateAsync(user, cancellationToken);
            throw new InvalidCredentialsException();
        }

        // Check if account is locked
        if (user.IsLocked)
        {
            _logger.LogWarning("Login failed - account locked for user: {UserId}", user.Id);
            throw new AccountLockedException(user.LockedUntil ?? DateTime.UtcNow.AddMinutes(30));
        }

        // Reset failed login attempts on successful password verification
        user.ResetFailedLoginAttempts();

        // Get user roles and permissions
        var userRoles = await _userRoleRepository.GetUserRolesAsync(user.Id, cancellationToken);
        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

        var roles = userRoles.Select(ur => ur.Role.Name).ToList();
        var permissionNames = permissions.ToList();

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user, roles, permissionNames);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Update user's refresh token
        await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, cancellationToken);

        _logger.LogInformation("Login successful for user: {UserId}", user.Id);

        return new AuthResponse
        {
            AccessToken = accessToken.Token,
            RefreshToken = refreshToken,
            ExpiresAt = accessToken.ExpiresAt,
            User = user.ToDto(),
            Roles = roles,
            Permissions = permissionNames
        };
    }
}
