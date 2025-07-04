using AutoMapper;
using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Exceptions;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bwadl.Accounting.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository, 
        IPasswordService passwordService,
        IMapper mapper, 
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting user creation for email: {Email}", request.Email);

        // Check if user with email already exists
        if (!string.IsNullOrWhiteSpace(request.Email) && 
            await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            _logger.LogWarning("User creation failed - email already exists: {Email}", request.Email);
            throw new DuplicateEmailException(request.Email);
        }

        // Create mobile if provided
        Mobile? mobile = null;
        if (!string.IsNullOrWhiteSpace(request.MobileNumber))
        {
            mobile = new Mobile(request.MobileNumber, request.MobileCountryCode ?? "+966");
        }

        // Create identity if provided
        Identity? identity = null;
        if (!string.IsNullOrWhiteSpace(request.IdentityId) && !string.IsNullOrWhiteSpace(request.IdentityType))
        {
            identity = request.IdentityType.ToLower() switch
            {
                "nid" => Identity.CreateNationalId(request.IdentityId),
                "gccid" => Identity.CreateGccId(request.IdentityId),
                "iqm" => Identity.CreateIqama(request.IdentityId),
                _ => Identity.CreateNationalId(request.IdentityId)
            };
        }

        // Create language enum
        var language = !string.IsNullOrWhiteSpace(request.Language) 
            ? LanguageExtensions.FromCode(request.Language) 
            : Language.English;

        _logger.LogInformation("Creating new user with email: {Email}", request.Email);

        var user = new User(request.Email, mobile, identity, request.NameEn, request.NameAr, language);
        
        // Set password if provided
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var hashedPassword = _passwordService.HashPassword(request.Password);
            user.SetPassword(request.Password, hashedPassword);
        }
        
        _logger.LogInformation("Saving user to repository");
        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);
        return _mapper.Map<UserDto>(createdUser);
    }
}
