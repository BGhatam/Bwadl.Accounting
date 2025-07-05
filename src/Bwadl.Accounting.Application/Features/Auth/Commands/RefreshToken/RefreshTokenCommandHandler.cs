using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Exceptions;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IUserRoleRepository userRoleRepository,
        IPermissionService permissionService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _userRoleRepository = userRoleRepository;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refresh token request received");

        // Validate refresh token
        if (!_jwtService.ValidateRefreshToken(request.RefreshToken))
        {
            _logger.LogWarning("Invalid refresh token provided");
            throw new InvalidRefreshTokenException();
        }

        // Find user by refresh token
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found for refresh token");
            throw new InvalidRefreshTokenException();
        }

        // Check if user account is still active
        if (user.IsLocked)
        {
            _logger.LogWarning("Refresh token request for locked account: {UserId}", user.Id);
            throw new AccountLockedException(user.LockedUntil ?? DateTime.UtcNow.AddMinutes(30));
        }

        // Get user roles and permissions
        var userRoles = await _userRoleRepository.GetUserRolesAsync(user.Id, cancellationToken);
        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

        var roles = userRoles.Select(ur => ur.Role.Name).ToList();
        var permissionNames = permissions.ToList();

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(user, roles, permissionNames);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Update user's refresh token
        await _userRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken, cancellationToken);

        _logger.LogInformation("Refresh token successful for user: {UserId}", user.Id);

        return new AuthResponse
        {
            AccessToken = accessToken.Token,
            RefreshToken = newRefreshToken,
            ExpiresAt = accessToken.ExpiresAt,
            User = user.ToUserDto(),
            Roles = roles,
            Permissions = permissionNames
        };
    }
}
