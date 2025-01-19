using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Mappings
{
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

                .ForMember(dest => dest.Name, opt => opt.MapFrom((src, dest) => src.Name ?? dest.Name))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom((src, dest) => src.Weight ?? dest.Weight))
                .ForMember(dest => dest.Height, opt => opt.MapFrom((src, dest) => src.Height ?? dest.Height))
                .ForMember(dest => dest.ActivityLevel, opt => opt.MapFrom(
                    (src, dest) => src.ActivityLevel.HasValue ? src.ActivityLevel : dest.ActivityLevel))
                .ForMember(dest => dest.DesiredGlassesOfWater, opt => opt.MapFrom(
                    (src, dest) => src.DesiredGlassesOfWater ?? dest.DesiredGlassesOfWater));
        }
    }
}
