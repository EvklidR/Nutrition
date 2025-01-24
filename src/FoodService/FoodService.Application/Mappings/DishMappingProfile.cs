using FoodService.Application.DTOs;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mapping
{
    public class DishMappingProfile : AutoMapper.Profile
    {
        public DishMappingProfile()
        {
            CreateMap<CreateDishDTO, Dish>()
                .ForMember(dest => dest.WeightOfPortion, opt => opt.MapFrom(src =>
                    src.Ingredients.Sum(i => i.Weight) / src.AmountOfPortions));

            CreateMap<UpdateDishDTO, Dish>()
                .ForMember(dest => dest.WeightOfPortion, opt => opt.MapFrom(src =>
                    src.Ingredients.Sum(i => i.Weight) / src.AmountOfPortions));


            CreateMap<ProductOfDish, ProductOfDish>();
        }
    }
}
