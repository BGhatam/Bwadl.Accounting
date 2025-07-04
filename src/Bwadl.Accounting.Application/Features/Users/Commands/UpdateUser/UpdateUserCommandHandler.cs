using AutoMapper;
using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Exceptions;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper, ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting user update for ID: {UserId}", request.Id);

        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User update failed - user not found with ID: {UserId}", request.Id);
            throw new UserNotFoundException(request.Id.ToString());
        }

        _logger.LogInformation("Found user {UserId}, checking for email conflicts", request.Id);

        // Check if email is being changed and if it already exists
        if (!string.IsNullOrWhiteSpace(request.Email) && 
            user.Email != request.Email && 
            await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            _logger.LogWarning("User update failed - email already exists: {Email} for user {UserId}", request.Email, request.Id);
            throw new DuplicateEmailException(request.Email);
        }

        _logger.LogInformation("Updating user {UserId}", request.Id);

        // Update email if provided
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            user.UpdateEmail(request.Email);
        }

        // Update mobile if provided
        if (!string.IsNullOrWhiteSpace(request.MobileNumber))
        {
            var mobile = new Mobile(request.MobileNumber, request.MobileCountryCode ?? "+966");
            user.UpdateMobile(mobile);
        }

        // Update identity if provided
        if (!string.IsNullOrWhiteSpace(request.IdentityId) && !string.IsNullOrWhiteSpace(request.IdentityType))
        {
            var identity = request.IdentityType.ToLower() switch
            {
                "nid" => Identity.CreateNationalId(request.IdentityId),
                "gccid" => Identity.CreateGccId(request.IdentityId),
                "iqm" => Identity.CreateIqama(request.IdentityId),
                _ => Identity.CreateNationalId(request.IdentityId)
            };
            user.UpdateIdentity(identity);
        }

        // Update names
        user.UpdateNames(request.NameEn, request.NameAr);

        // Update language if provided
        if (!string.IsNullOrWhiteSpace(request.Language))
        {
            var language = LanguageExtensions.FromCode(request.Language);
            user.UpdateLanguage(language);
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("User {UserId} updated successfully", request.Id);
        return _mapper.Map<UserDto>(user);
    }
}
