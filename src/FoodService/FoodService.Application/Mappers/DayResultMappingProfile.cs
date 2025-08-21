using AutoMapper;
using FoodService.Application.DTOs.DayResult.Requests;
using FoodService.Application.DTOs.DayResult.Responses;
using FoodService.Application.Helpers;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappers
{
    public class DayResultMappingProfile : Profile
    {
        public DayResultMappingProfile()
        {
            CreateMap<UpdateDayResultDTO, DayResult>();

            CreateMap<DayResult, DayResultResponse>();

            CreateMap<DayResult, ShortDayResultResponse>()
                .ForMember(d => d.Calories, opt => opt.MapFrom(s => CalculationHelper.CalculateCaloriesOfDayResult(s)))
                .ForMember(d => d.Proteins, opt => opt.MapFrom(s => CalculationHelper.CalculateProteinsOfDayResult(s)))
                .ForMember(d => d.Fats, opt => opt.MapFrom(s => CalculationHelper.CalculateFatsOfDayResult(s)))
                .ForMember(d => d.Carbohydrates, opt => opt.MapFrom(s => CalculationHelper.CalculateCarbohydratesOfDayResult(s)));
        }
    }
}