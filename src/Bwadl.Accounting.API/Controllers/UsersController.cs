using Asp.Versioning;
using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Application.Features.Users.Commands.CreateUser;
using Bwadl.Accounting.Application.Features.Users.Commands.DeleteUser;
using Bwadl.Accounting.Application.Features.Users.Commands.UpdateUser;
using Bwadl.Accounting.API.Models.Users;
using Bwadl.Accounting.Application.Features.Users.Queries.GetAllUsers;
using Bwadl.Accounting.Application.Features.Users.Queries.GetUser;
using Bwadl.Accounting.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bwadl.Accounting.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
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
    public async Task<ActionResult<PagedResponse<UserDto>>> GetAllUsers(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET /api/users - Retrieving users with pagination. Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var query = new GetAllUsersQuery();
        var users = await _mediator.Send(query, cancellationToken);
        
        var userList = users.ToList();

        // Apply pagination
        var totalCount = userList.Count;
        var pagedUsers = userList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var pagedResponse = new PagedResponse<UserDto>
        {
            Data = pagedUsers,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };

        _logger.LogInformation("GET /api/users - Returning {UserCount} users (Page {Page} of {TotalPages})", 
            pagedUsers.Count, page, pagedResponse.TotalPages);

        return Ok(pagedResponse);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDetailResponse>> GetUser(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET /api/users/{UserId} - Retrieving user by ID", id);

        var query = new GetUserQuery(id);
        var user = await _mediator.Send(query, cancellationToken);

        if (user == null)
        {
            _logger.LogInformation("GET /api/users/{UserId} - User not found", id);
            return NotFound($"User with ID {id} not found.");
        }

        // Add enhanced metadata
        var response = new UserDetailResponse
        {
            User = user,
            Metadata = new UserMetadata
            {
                ProfileCompleteness = CalculateProfileCompleteness(user),
                LastLoginDays = CalculateLastLoginDays(user),
                AccountAge = DateTime.UtcNow - user.CreatedAt
            }
        };

        _logger.LogInformation("GET /api/users/{UserId} - Returning user with metadata successfully", id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] Models.Users.CreateUserRequest request, CancellationToken cancellationToken)
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

            _logger.LogInformation("POST /api/users - User created successfully with ID: {UserId}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (DuplicateEmailException ex)
        {
            _logger.LogWarning("POST /api/users - Failed to create user due to duplicate email: {Email}", request.Email);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] Models.Users.UpdateUserRequest request, CancellationToken cancellationToken)
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

            _logger.LogInformation("PUT /api/users/{UserId} - User updated successfully", id);
            return Ok(user);
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

    private static double CalculateProfileCompleteness(UserDto user)
    {
        // Calculate profile completeness based on key fields
        var fields = new[] { user.NameEn, user.NameAr, user.Email, user.MobileNumber, user.IdentityId };
        var completedFields = fields.Count(f => !string.IsNullOrWhiteSpace(f));
        return (double)completedFields / fields.Length * 100;
    }

    private static int CalculateLastLoginDays(UserDto user)
    {
        // Mock calculation - in real app, you'd have LastLogin property
        // For now, use days since account creation as a placeholder
        return (DateTime.UtcNow - user.CreatedAt).Days;
    }
}
