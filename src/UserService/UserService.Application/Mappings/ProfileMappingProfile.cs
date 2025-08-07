using UserService.Application.DTOs.Requests.Profile;
using UserService.Application.DTOs.Responses.Profile;
using UserService.Domain.Entities;

namespace UserService.Application.Mappings;

public class ProfileMappingProfile : AutoMapper.Profile
{
    public ProfileMappingProfile()
    {
        CreateMap<CreateProfileDTO, Profile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UpdateProfileDTO, Profile>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Birthday, opt => opt.Ignore())
            .ForMember(dest => dest.Gender, opt => opt.Ignore())
            .ForMember(dest => dest.ThereIsMealPlan, opt => opt.Ignore())

            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
            .ForMember(dest => dest.ActivityLevel, opt => opt.MapFrom(src => src.ActivityLevel));

        CreateMap<Profile, ShortProfileResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

    }
}
