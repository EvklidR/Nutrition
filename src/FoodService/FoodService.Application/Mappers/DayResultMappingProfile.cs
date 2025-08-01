using AutoMapper;
using FoodService.Application.DTOs.DayResult.Requests;
using FoodService.Application.DTOs.DayResult.Responses;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappers
{
    public class DayResultMappingProfile : Profile
    {
        public DayResultMappingProfile()
        {
            CreateMap<CreateDayResultDTO, DayResult>();

            CreateMap<UpdateDayResultDTO, DayResult>();

            CreateMap<DayResult, DayResultResponse>();

            CreateMap<DayResult, ShortDayResultResponse>()
                .ForMember(d => d.Calories, opt => opt.MapFrom(s => 
                    s.Meals.SelectMany(dr => dr.Products.Select(p => p.Food.Calories)).Sum() +
                    s.Meals.SelectMany(dr => dr.Dishes.Select(p => p.Food.Calories)).Sum()))
                .ForMember(d => d.Proteins, opt => opt.MapFrom(s => 
                    s.Meals.SelectMany(dr => dr.Products.Select(p => p.Food.Proteins)).Sum() +
                    s.Meals.SelectMany(dr => dr.Dishes.Select(p => p.Food.Proteins)).Sum()))
                .ForMember(d => d.Fats, opt => opt.MapFrom(s => 
                    s.Meals.SelectMany(dr => dr.Products.Select(p => p.Food.Fats)).Sum() +
                    s.Meals.SelectMany(dr => dr.Dishes.Select(p => p.Food.Fats)).Sum()))
                .ForMember(d => d.Carbohydrates, opt => opt.MapFrom(s => 
                    s.Meals.SelectMany(dr => dr.Products.Select(p => p.Food.Carbohydrates)).Sum() + 
                    s.Meals.SelectMany(dr => dr.Dishes.Select(p => p.Food.Carbohydrates)).Sum()));
        }
    }
}