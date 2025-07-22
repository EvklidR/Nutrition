using AutoMapper;
using FoodService.Application.DTOs.Dish;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappers
{
    public class DishMappingProfile : Profile
    {
        public DishMappingProfile()
        {
            CreateMap<Dish, DishResponse>();
        }
    }
}