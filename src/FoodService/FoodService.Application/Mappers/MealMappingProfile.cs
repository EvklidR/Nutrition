using AutoMapper;
using FoodService.Application.DTOs.Meal.Requests;
using FoodService.Application.DTOs.Meal.Responses;
using FoodService.Application.Helpers;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappers;

public class MealMappingProfile : Profile
{
    public MealMappingProfile()
    {
        CreateMap<CreateMealDTO, Meal>();

        CreateMap<UpdateMealDTO, Meal>();

        CreateMap<Meal, ShortMealResponse>()
            .ForMember(dest => dest.TotalCalories, opt => opt.MapFrom(src => CalculationHelper.CalculateCaloriesOfMeal(src)))
            .ForMember(dest => dest.TotalProteins, opt => opt.MapFrom(src => CalculationHelper.CalculateProteinsOfMeal(src)))
            .ForMember(dest => dest.TotalCarbohydrates, opt => opt.MapFrom(src => CalculationHelper.CalculateCarbohydratesOfMeal(src)))
            .ForMember(dest => dest.TotalFats, opt => opt.MapFrom(src => CalculationHelper.CalculateFatsOfMeal(src)));

        CreateMap<Meal, FullMealResponse>()
            .ForMember(dest => dest.TotalCalories, opt => opt.MapFrom(src => CalculationHelper.CalculateCaloriesOfMeal(src)))
            .ForMember(dest => dest.TotalProteins, opt => opt.MapFrom(src => CalculationHelper.CalculateProteinsOfMeal(src)))
            .ForMember(dest => dest.TotalCarbohydrates, opt => opt.MapFrom(src => CalculationHelper.CalculateCarbohydratesOfMeal(src)))
            .ForMember(dest => dest.TotalFats, opt => opt.MapFrom(src => CalculationHelper.CalculateFatsOfMeal(src)));

        CreateMap<EatenProduct, MealProductResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductCalories, opt => opt.MapFrom(src => src.Product.Calories))
            .ForMember(dest => dest.ProductProteins, opt => opt.MapFrom(src => src.Product.Proteins))
            .ForMember(dest => dest.ProductCarbohydrates, opt => opt.MapFrom(src => src.Product.Carbohydrates))
            .ForMember(dest => dest.ProductFats, opt => opt.MapFrom(src => src.Product.Fats));

        CreateMap<EatenDish, MealDishResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DishId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Dish.Name))
            .ForMember(dest => dest.ProductCalories, opt => opt.MapFrom(src => src.Dish.Calories))
            .ForMember(dest => dest.ProductProteins, opt => opt.MapFrom(src => src.Dish.Proteins))
            .ForMember(dest => dest.ProductCarbohydrates, opt => opt.MapFrom(src => src.Dish.Carbohydrates))
            .ForMember(dest => dest.ProductFats, opt => opt.MapFrom(src => src.Dish.Fats))
            .ForMember(dest => dest.WeightOfPortion, opt => opt.MapFrom(src => src.Dish.WeightOfPortion));

        CreateMap<CreateOrUpdateEatenDishDTO, EatenDish>()
            .ForMember(dest => dest.DishId, opt => opt.MapFrom(src => src.FoodId))
            .ForMember(dest => dest.AmountOfPortions, opt => opt.MapFrom(src => src.AmountOfPortions));
        
        CreateMap<CreateOrUpdateEatenProductDTO, EatenProduct>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.FoodId))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight));
    }
}