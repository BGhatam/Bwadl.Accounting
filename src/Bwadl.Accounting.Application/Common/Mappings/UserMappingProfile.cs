using AutoMapper;
using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Entities;

namespace Bwadl.Accounting.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}
