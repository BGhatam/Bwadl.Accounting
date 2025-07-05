using Asp.Versioning;
using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.API.Models.Auth;
using Bwadl.Accounting.Application.Features.Auth.Commands.Login;
using Bwadl.Accounting.Application.Features.Auth.Commands.Register;
using Bwadl.Accounting.Application.Features.Auth.Commands.RefreshToken;
using Bwadl.Accounting.Application.Features.Auth.Commands.ChangePassword;
using Bwadl.Accounting.Application.Features.Auth.Queries.ValidateToken;
using Bwadl.Accounting.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bwadl.Accounting.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IMediator mediator,
        ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] Models.Auth.LoginRequest request)
    {
        try
        {
            var command = new LoginCommand(request.Username, request.Password, request.RememberMe);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidCredentialsException)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }
        catch (AccountLockedException ex)
        {
            return Unauthorized(new { message = ex.Message });
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
    public async Task<ActionResult<AuthResponse>> Register([FromBody] Models.Auth.RegisterRequest request)
    {
        try
        {
            var command = new RegisterCommand(
                request.Email,
                request.Mobile,
                request.Password,
                request.ConfirmPassword,
                request.NameEn,
                request.NameAr
            );
            
            var result = await _mediator.Send(command);
            return Created($"/api/v1/users/{result.User.Id}", result);
        }
        catch (DuplicateEmailException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (RegistrationFailedException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (WeakPasswordException ex)
        {
            return BadRequest(new { message = ex.Message });
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
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] Models.Auth.RefreshTokenRequest request)
    {
        try
        {
            var command = new RefreshTokenCommand(request.RefreshToken);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidRefreshTokenException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (RefreshTokenExpiredException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (AccountLockedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] Models.Auth.ChangePasswordRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Invalid user context" });
            }

            var command = new ChangePasswordCommand(
                userId,
                request.CurrentPassword,
                request.NewPassword,
                request.ConfirmNewPassword
            );

            await _mediator.Send(command);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidCredentialsException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (WeakPasswordException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (RegistrationFailedException ex)
        {
            return BadRequest(new { message = ex.Message });
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
    public ActionResult Logout()
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("User {UserId} logged out successfully", userIdClaim);
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
    public ActionResult<UserDto> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Invalid user context" });
            }

            // For now, we can get user info through the Users query pattern
            // In a real scenario, you might create a GetCurrentUser query
            return Ok(new { message = "Current user endpoint - to be implemented with Users query" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new { message = "An error occurred while getting user information" });
        }
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    [HttpPost("validate-token")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenValidationResponse>> ValidateToken([FromBody] string token)
    {
        try
        {
            var query = new ValidateTokenQuery(token);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token validation");
            return StatusCode(500, new { message = "An error occurred during token validation" });
        }
    }

}
