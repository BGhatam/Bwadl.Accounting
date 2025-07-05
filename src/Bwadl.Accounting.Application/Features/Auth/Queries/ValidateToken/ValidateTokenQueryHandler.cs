using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Auth.Queries.ValidateToken;

public class ValidateTokenQueryHandler : IRequestHandler<ValidateTokenQuery, TokenValidationResponse>
{
    private readonly IJwtService _jwtService;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<ValidateTokenQueryHandler> _logger;

    public ValidateTokenQueryHandler(
        IJwtService jwtService,
        IUserRoleRepository userRoleRepository,
        IPermissionService permissionService,
        ILogger<ValidateTokenQueryHandler> logger)
    {
        _jwtService = jwtService;
        _userRoleRepository = userRoleRepository;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<TokenValidationResponse> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Token validation request received");

        try
        {
            var tokenValidation = _jwtService.ValidateAccessToken(request.Token);
            
            if (!tokenValidation.IsValid)
            {
                _logger.LogWarning("Token validation failed - invalid token");
                return new TokenValidationResponse { IsValid = false };
            }

            var userId = int.Parse(tokenValidation.UserId!);

            // Get fresh user roles and permissions
            var userRoles = await _userRoleRepository.GetUserRolesAsync(userId, cancellationToken);
            var permissions = await _permissionService.GetUserPermissionsAsync(userId, cancellationToken);

            var roles = userRoles.Select(ur => ur.Role.Name).ToList();
            var permissionNames = permissions.ToList();

            _logger.LogInformation("Token validation successful for user: {UserId}", userId);

            return new TokenValidationResponse
            {
                IsValid = true,
                UserId = tokenValidation.UserId,
                Roles = roles,
                Permissions = permissionNames,
                ExpiresAt = tokenValidation.ExpiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token validation");
            return new TokenValidationResponse { IsValid = false };
        }
    }
}
