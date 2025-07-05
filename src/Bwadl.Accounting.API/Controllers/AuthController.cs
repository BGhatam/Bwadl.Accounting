using Asp.Versioning;
using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Application.Common.Extensions;
using Bwadl.Accounting.Application.Features.Auth.DTOs;
using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bwadl.Accounting.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtService jwtService,
        IUserRoleRepository userRoleRepository,
        IPermissionService permissionService,
        ILogger<AuthController> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _userRoleRepository = userRoleRepository;
        _permissionService = permissionService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Find user by email or mobile
            User? user = null;
            
            if (request.Username.Contains("@"))
            {
                user = await _userRepository.GetByEmailAsync(request.Username);
            }
            else
            {
                // Try to parse as mobile number
                if (Mobile.TryParse(request.Username, out var mobile) && mobile != null)
                {
                    user = await _userRepository.GetByMobileAsync(mobile.Number, mobile.CountryCode);
                }
            }

            if (user == null || !user.ComparePassword(request.Password, _passwordService))
            {
                _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Get user roles and permissions
            var roles = await _userRoleRepository.GetUserRoleNamesAsync(user.Id);
            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

            // Generate JWT token
            var accessToken = _jwtService.GenerateToken(user, roles, permissions);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.Add(_jwtService.AccessTokenExpiration);

            // Update session ID (you might want to store refresh token in database)
            user.UpdateSessionId(Guid.NewGuid().ToString());
            await _userRepository.UpdateAsync(user);

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = user.ToDto(),
                Roles = roles,
                Permissions = permissions
            };

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for username: {Username}", request.Username);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Validate that at least email or mobile is provided
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Mobile))
            {
                return BadRequest(new { message = "Either email or mobile number is required" });
            }

            // Check if user already exists
            if (!string.IsNullOrEmpty(request.Email))
            {
                var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUserByEmail != null)
                {
                    return Conflict(new { message = "User with this email already exists" });
                }
            }

            Mobile? mobile = null;
            if (!string.IsNullOrEmpty(request.Mobile))
            {
                if (!Mobile.TryParse(request.Mobile, out mobile) || mobile == null)
                {
                    return BadRequest(new { message = "Invalid mobile number format" });
                }

                var existingUserByMobile = await _userRepository.GetByMobileAsync(mobile.Number, mobile.CountryCode);
                if (existingUserByMobile != null)
                {
                    return Conflict(new { message = "User with this mobile number already exists" });
                }
            }

            // Create new user
            var user = new User(request.Email, mobile, null, request.NameEn, request.NameAr);
            var hashedPassword = _passwordService.HashPassword(request.Password);
            user.SetPassword(request.Password, hashedPassword);

            await _userRepository.CreateAsync(user);

            // Assign default user role (you might want to create this role if it doesn't exist)
            var userRole = new UserRole(user.Id, 3); // Assuming User role has ID 3
            await _userRoleRepository.AssignRoleAsync(userRole);

            // Get user roles and permissions
            var roles = await _userRoleRepository.GetUserRoleNamesAsync(user.Id);
            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

            // Generate JWT token
            var accessToken = _jwtService.GenerateToken(user, roles, permissions);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.Add(_jwtService.AccessTokenExpiration);

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = user.ToDto(),
                Roles = roles,
                Permissions = permissions
            };

            _logger.LogInformation("User {UserId} registered successfully", user.Id);
            return Created($"/api/v1/users/{user.Id}", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        // Note: In a production system, you would validate the refresh token against stored tokens
        // For now, we'll return a simple error message
        return Task.FromResult<ActionResult<AuthResponse>>(BadRequest(new { message = "Refresh token functionality not yet implemented" }));
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Invalid user context" });
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Verify current password
            if (!user.ComparePassword(request.CurrentPassword, _passwordService))
            {
                return BadRequest(new { message = "Current password is incorrect" });
            }

            // Update password
            var newHashedPassword = _passwordService.HashPassword(request.NewPassword);
            user.SetPassword(request.NewPassword, newHashedPassword);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} changed password successfully", userId);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change");
            return StatusCode(500, new { message = "An error occurred while changing password" });
        }
    }

    /// <summary>
    /// Logout user (invalidate token)
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out var userId))
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    // Clear session ID
                    user.UpdateSessionId(null);
                    await _userRepository.UpdateAsync(user);
                }
            }

            _logger.LogInformation("User {UserId} logged out successfully", userId);
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { message = "An error occurred during logout" });
        }
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Invalid user context" });
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new { message = "An error occurred while getting user information" });
        }
    }

}
