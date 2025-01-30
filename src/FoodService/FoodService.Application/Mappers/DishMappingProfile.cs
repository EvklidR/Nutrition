using AutoMapper;
using FoodService.Application.DTOs.Dish;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mapping
{
    public class DishMappingProfile : Profile
    {
        public DishMappingProfile()
        {
            CreateMap<CreateDishDTO, Dish>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src =>
                    src.Ingredients.Select(i => new ProductOfDish
                    {
                        ProductId = i.ProductId,
                        WeightPerPortion = i.Weight / src.AmountOfPortions
                    }).ToList()));

            CreateMap<UpdateDishDTO, Dish>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src =>
                    src.Ingredients.Select(i => new ProductOfDish
                    {
                        ProductId = i.ProductId,
                        WeightPerPortion = i.Weight / src.AmountOfPortions
                    }).ToList()));

            CreateMap<ProductOfDish, DishProductDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Calories, opt => opt.MapFrom(src => src.Product.Calories))
                .ForMember(dest => dest.Proteins, opt => opt.MapFrom(src => src.Product.Proteins))
                .ForMember(dest => dest.Fats, opt => opt.MapFrom(src => src.Product.Fats))
                .ForMember(dest => dest.Carbohydrates, opt => opt.MapFrom(src => src.Product.Carbohydrates));

            CreateMap<Dish, FullDishDishDTO>()
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => 
                    src.Ingredients.Select(i => i.WeightPerPortion).Sum()));

            CreateMap<Dish, BriefDishDishDTO>()
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src =>
                    src.Ingredients.Select(i => i.WeightPerPortion).Sum()));
        }
    }
}