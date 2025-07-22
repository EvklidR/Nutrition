using AutoMapper;
using FoodService.Application.DTOs.Recipe.Requests;
using FoodService.Application.DTOs.Recipe.Responses;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappers;

public class RecipeMappingProfile : Profile
{
    public RecipeMappingProfile()
    {
        CreateMap<Recipe, RecipeResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Dish.Name));

        CreateMap<Recipe, CalculatedRecipeResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Dish.Name))
            .ForMember(dest => dest.Calories, opt => opt.MapFrom(src => src.Dish.Calories))
            .ForMember(dest => dest.Carbohydrates, opt => opt.MapFrom(src => src.Dish.Carbohydrates))
            .ForMember(dest => dest.Proteins, opt => opt.MapFrom(src => src.Dish.Proteins))
            .ForMember(dest => dest.Fats, opt => opt.MapFrom(src => src.Dish.Fats));

        CreateMap<ProductOfRecipe, RecipeProductResponse>();

        CreateMap<CreateRecipeDTO, Recipe>();
        CreateMap<UpdateRecipeDTO, Recipe>();
        CreateMap<CreateOrUpdateProductOfRecipeDTO, ProductOfRecipe>();
    }
}
