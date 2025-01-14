using AutoMapper;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.Core.Entities;

namespace MealPlanService.BusinessLogic.Mappers
{
    public class ProfilePlanProfile : Profile
    {
        public ProfilePlanProfile() 
        {
            CreateMap<ProfileMealPlanDTO, ProfileMealPlan>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Now)))
                .ForMember(dest => dest.EndDate, opt => opt.Ignore());
        }
    }
}
