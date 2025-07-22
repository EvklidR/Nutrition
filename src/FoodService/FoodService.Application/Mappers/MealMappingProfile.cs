using AutoMapper;
using FoodService.Application.DTOs.Meal.Requests;
using FoodService.Application.DTOs.Meal.Responses;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappers;

public class MealMappingProfile : Profile
{
    public MealMappingProfile()
    {
        CreateMap<CreateMealDTO, Meal>();

        CreateMap<UpdateMealDTO, Meal>();

        CreateMap<CreateOrUpdateEatenDishDTO, EatenDish>();
        CreateMap<CreateOrUpdateEatenProductDTO, EatenProduct>();
    }
}