using AutoMapper;
using FoodService.Application.DTOs;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappings
{
    public class ProductMappingProfile : AutoMapper.Profile
    {
        public ProductMappingProfile() 
        {
            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest =>  dest.Id, opt => opt.Ignore())
                .ForMember(
                dest => dest.Calories, 
                opt => opt.MapFrom(src => src.Carbohydrates * 4 + src.Proteins * 4 + src.Fats * 9));

            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(
                dest => dest.Calories, 
                opt => opt.MapFrom(src => src.Carbohydrates * 4 + src.Proteins * 4 + src.Fats * 9));

        }
    }
}
