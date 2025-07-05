using Bwadl.Accounting.Application.Common.DTOs;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string? Email,
    string? Mobile,
    string Password,
    string ConfirmPassword,
    string? NameEn,
    string? NameAr
) : IRequest<AuthResponse>;
