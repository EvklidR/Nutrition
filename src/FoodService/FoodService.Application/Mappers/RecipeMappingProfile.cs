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

        CreateMap<ProductOfRecipe, RecipeProductResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Calories, opt => opt.MapFrom(src => src.Product.Calories))
            .ForMember(dest => dest.Proteins, opt => opt.MapFrom(src => src.Product.Proteins))
            .ForMember(dest => dest.Carbohydrates, opt => opt.MapFrom(src => src.Product.Carbohydrates))
            .ForMember(dest => dest.Fats, opt => opt.MapFrom(src => src.Product.Fats))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.WeightInRecipe));

        CreateMap<CreateRecipeDTO, Recipe>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

        CreateMap<UpdateRecipeDTO, Recipe>()
            .ForPath(dest => dest.Dish.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

        CreateMap<CreateOrUpdateProductOfRecipeDTO, ProductOfRecipe>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.WeightInRecipe, opt => opt.MapFrom(src => src.WeightInRecipe));
    }
}
