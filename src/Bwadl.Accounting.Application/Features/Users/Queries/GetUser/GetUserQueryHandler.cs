using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserQueryHandler> _logger;

    public GetUserQueryHandler(IUserRepository userRepository, ILogger<GetUserQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving user with ID: {UserId}", request.Id);

        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (user == null)
        {
            _logger.LogInformation("User not found with ID: {UserId}", request.Id);
            return null;
        }

        _logger.LogInformation("User found with ID: {UserId}, Display Name: {DisplayName}", user.Id, user.GetDisplayName());
        return user.ToDto();
    }
}
