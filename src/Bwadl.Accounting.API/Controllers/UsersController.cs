using Asp.Versioning;
using Bwadl.Accounting.API.Models.Requests;
using Bwadl.Accounting.API.Models.Responses;
using Bwadl.Accounting.Application.Features.Users.Commands.CreateUser;
using Bwadl.Accounting.Application.Features.Users.Commands.DeleteUser;
using Bwadl.Accounting.Application.Features.Users.Commands.UpdateUser;
using Bwadl.Accounting.Application.Features.Users.Queries.GetAllUsers;
using Bwadl.Accounting.Application.Features.Users.Queries.GetUser;
using Bwadl.Accounting.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bwadl.Accounting.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();
        var users = await _mediator.Send(query, cancellationToken);
        
        var response = users.Select(user => new UserResponse(
            user.Id,
            user.Email,
            user.MobileNumber,
            user.MobileCountryCode,
            user.IdentityId,
            user.IdentityType,
            user.NameEn,
            user.NameAr,
            user.Language,
            user.IsEmailVerified,
            user.IsMobileVerified,
            user.IsUserVerified,
            user.EmailVerifiedAt,
            user.MobileVerifiedAt,
            user.UserVerifiedAt,
            user.CreatedAt,
            user.UpdatedAt
        ));

        var userList = response.ToList();
        _logger.LogInformation("GET /api/users - Returning {UserCount} users", userList.Count);
        return Ok(userList);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetUser(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET /api/users/{UserId} - Retrieving user by ID", id);

        var query = new GetUserQuery(id);
        var user = await _mediator.Send(query, cancellationToken);

        if (user == null)
        {
            _logger.LogInformation("GET /api/users/{UserId} - User not found", id);
            return NotFound($"User with ID {id} not found.");
        }

        var response = new UserResponse(
            user.Id,
            user.Email,
            user.MobileNumber,
            user.MobileCountryCode,
            user.IdentityId,
            user.IdentityType,
            user.NameEn,
            user.NameAr,
            user.Language,
            user.IsEmailVerified,
            user.IsMobileVerified,
            user.IsUserVerified,
            user.EmailVerifiedAt,
            user.MobileVerifiedAt,
            user.UserVerifiedAt,
            user.CreatedAt,
            user.UpdatedAt
        );

        _logger.LogInformation("GET /api/users/{UserId} - Returning user successfully", id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateUserCommand(
                request.Email,
                request.MobileNumber,
                request.MobileCountryCode,
                request.IdentityId,
                request.IdentityType,
                request.NameEn,
                request.NameAr,
                request.Language,
                request.Password
            );
            var user = await _mediator.Send(command, cancellationToken);

            var response = new UserResponse(
                user.Id,
                user.Email,
                user.MobileNumber,
                user.MobileCountryCode,
                user.IdentityId,
                user.IdentityType,
                user.NameEn,
                user.NameAr,
                user.Language,
                user.IsEmailVerified,
                user.IsMobileVerified,
                user.IsUserVerified,
                user.EmailVerifiedAt,
                user.MobileVerifiedAt,
                user.UserVerifiedAt,
                user.CreatedAt,
                user.UpdatedAt
            );

            _logger.LogInformation("POST /api/users - User created successfully with ID: {UserId}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
        }
        catch (DuplicateEmailException ex)
        {
            _logger.LogWarning("POST /api/users - Failed to create user due to duplicate email: {Email}", request.Email);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("PUT /api/users/{UserId} - Updating user", id);

        try
        {
            var command = new UpdateUserCommand(
                id,
                request.Email,
                request.MobileNumber,
                request.MobileCountryCode,
                request.IdentityId,
                request.IdentityType,
                request.NameEn,
                request.NameAr,
                request.Language
            );
            var user = await _mediator.Send(command, cancellationToken);

            var response = new UserResponse(
                user.Id,
                user.Email,
                user.MobileNumber,
                user.MobileCountryCode,
                user.IdentityId,
                user.IdentityType,
                user.NameEn,
                user.NameAr,
                user.Language,
                user.IsEmailVerified,
                user.IsMobileVerified,
                user.IsUserVerified,
                user.EmailVerifiedAt,
                user.MobileVerifiedAt,
                user.UserVerifiedAt,
                user.CreatedAt,
                user.UpdatedAt
            );

            _logger.LogInformation("PUT /api/users/{UserId} - User updated successfully", id);
            return Ok(response);
        }
        catch (UserNotFoundException ex)
        {
            _logger.LogWarning("PUT /api/users/{UserId} - User not found", id);
            return NotFound(ex.Message);
        }
        catch (DuplicateEmailException ex)
        {
            _logger.LogWarning("PUT /api/users/{UserId} - Failed to update user due to duplicate email: {Email}", id, request.Email);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DELETE /api/users/{UserId} - Deleting user", id);

        try
        {
            var command = new DeleteUserCommand(id);
            await _mediator.Send(command, cancellationToken);
            
            _logger.LogInformation("DELETE /api/users/{UserId} - User deleted successfully", id);
            return NoContent();
        }
        catch (UserNotFoundException ex)
        {
            _logger.LogWarning("DELETE /api/users/{UserId} - User not found", id);
            return NotFound(ex.Message);
        }
    }
}
