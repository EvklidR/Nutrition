using AutoMapper;
using FoodService.Application.DTOs;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappings
{
    public class DayResultMappingProfile : Profile
    {
        public DayResultMappingProfile()
        {
            CreateMap<UpdateDayResultDTO, DayResult>();


            CreateMap<CreateDayResultDTO, DayResult>();


            CreateMap<UpdateMealDTO, Meal>()
                .ForMember(dest => dest.Foods, opt => opt.MapFrom(src => src.Foods));

            CreateMap<CreateMealDTO, Meal>()
                .ForMember(dest => dest.Foods, opt => opt.MapFrom(src => src.Foods));

            CreateMap<CreateOrUpdateEatenFoodDTO, EatenFood>();
        }
    }
}
