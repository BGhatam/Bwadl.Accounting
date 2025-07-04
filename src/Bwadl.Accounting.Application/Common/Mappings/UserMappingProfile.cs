using AutoMapper;
using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.ValueObjects;

namespace Bwadl.Accounting.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.Mobile != null ? src.Mobile.Number : null))
            .ForMember(dest => dest.MobileCountryCode, opt => opt.MapFrom(src => src.Mobile != null ? src.Mobile.CountryCode : null))
            .ForMember(dest => dest.IdentityId, opt => opt.MapFrom(src => src.Identity != null ? src.Identity.Id : null))
            .ForMember(dest => dest.IdentityType, opt => opt.MapFrom(src => src.Identity != null ? src.Identity.Type.ToString().ToLowerInvariant() : null))
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language.ToCode()));
    }
}
