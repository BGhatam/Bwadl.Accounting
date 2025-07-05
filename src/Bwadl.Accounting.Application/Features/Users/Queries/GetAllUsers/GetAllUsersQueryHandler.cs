using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(IUserRepository userRepository, ILogger<GetAllUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all users");

        var users = await _userRepository.GetAllAsync(cancellationToken);
        var userList = users.ToList();
        
        _logger.LogInformation("Retrieved {UserCount} users", userList.Count);
        return userList.Select(u => u.ToDto());
    }
}
