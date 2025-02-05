using AutoMapper;
using FoodService.Application.DTOs.Meal;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappers
{
    public class MealMappingProfile : Profile
    {
        public MealMappingProfile()
        {
            CreateMap<CreateMealDTO, Meal>();

            CreateMap<UpdateMealDTO, Meal>();

            CreateMap<CreateOrUpdateEatenFoodDTO, EatenDish>();
            CreateMap<CreateOrUpdateEatenFoodDTO, EatenProduct>();

            CreateMap<Meal, BriefMealDTO>()
                .ForMember(dest => dest.TotalProteins, opt => opt.MapFrom(src =>
                    src.Products.Sum(ef => GetProteins(ef)) +
                    src.Dishes.Sum(ef => GetProteins(ef))))
                .ForMember(dest => dest.TotalCarbohydrates, opt => opt.MapFrom(src =>
                    src.Products.Sum(ef => GetCarbohydrates(ef)) +
                    src.Dishes.Sum(ef => GetCarbohydrates(ef))))
                .ForMember(dest => dest.TotalFats, opt => opt.MapFrom(src =>
                    src.Products.Sum(ef => GetFats(ef)) +
                    src.Dishes.Sum(ef => GetFats(ef))))
                .ForMember(dest => dest.TotalCalories, opt => opt.MapFrom(src =>
                    src.Products.Sum(ef => GetCalories(ef)) +
                    src.Dishes.Sum(ef => GetCalories(ef))));

            CreateMap<Meal, FullMealDTO>()
                .ForMember(dest => dest.TotalProteins, opt => opt.MapFrom(src =>
                    src.Products.Sum(ef => GetProteins(ef)) +
                    src.Dishes.Sum(ef => GetProteins(ef))))
                .ForMember(dest => dest.TotalCarbohydrates, opt => opt.MapFrom(src =>
                    src.Products.Sum(ef => GetCarbohydrates(ef)) +
                    src.Dishes.Sum(ef => GetCarbohydrates(ef))))
                .ForMember(dest => dest.TotalFats, opt => opt.MapFrom(src =>
                    src.Products.Sum(ef => GetFats(ef)) +
                    src.Dishes.Sum(ef => GetFats(ef))))
                .ForMember(dest => dest.TotalCalories, opt => opt.MapFrom(src =>
                    src.Products.Sum(ef => GetCalories(ef)) +
                    src.Dishes.Sum(ef => GetCalories(ef))));

            CreateMap<EatenProduct, MealFoodDTO>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Food.Id))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Food.Name))
                .ForMember(dest => dest.TotalProductProteins, 
                    opt => opt.MapFrom(src => GetProteins(src)))
                .ForMember(dest => dest.TotalProductFats,
                    opt => opt.MapFrom(src => GetFats(src)))
                .ForMember(dest => dest.TotalProductCarbohydrates, 
                    opt => opt.MapFrom(src => GetCarbohydrates(src)))
                .ForMember(dest => dest.TotalProductCalories, 
                    opt => opt.MapFrom(src => GetCalories(src)));

            CreateMap<EatenDish, MealFoodDTO>()
                .ForMember(dest => dest.Id, 
                    opt => opt.MapFrom(src => src.Food.Id))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Food.Name))
                .ForMember(dest => dest.TotalProductProteins,
                    opt => opt.MapFrom(src => GetProteins(src)))
                .ForMember(dest => dest.TotalProductFats,
                    opt => opt.MapFrom(src => GetFats(src)))
                .ForMember(dest => dest.TotalProductCarbohydrates,
                    opt => opt.MapFrom(src => GetCarbohydrates(src)))
                .ForMember(dest => dest.TotalProductCalories,
                    opt => opt.MapFrom(src => GetCalories(src)));
        }

        private double GetProteins(EatenFood eatenFood)
        {
            return eatenFood.Food.Proteins * eatenFood.Weight / 100;
        }

        private double GetFats(EatenFood eatenFood)
        {
            return eatenFood.Food.Fats * eatenFood.Weight / 100;
        }

        private double GetCarbohydrates(EatenFood eatenFood)
        {
            return eatenFood.Food.Carbohydrates * eatenFood.Weight / 100;
        }

        private double GetCalories(EatenFood eatenFood)
        {
            return eatenFood.Food.Calories * eatenFood.Weight / 100;
        }
    }
}