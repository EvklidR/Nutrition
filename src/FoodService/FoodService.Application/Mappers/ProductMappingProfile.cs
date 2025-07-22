using AutoMapper;
using FoodService.Application.DTOs.Product.Requests;
using FoodService.Application.DTOs.Product.Responses;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappers
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile() 
        {
            CreateMap<CreateProductDTO, Product>()
                .ForMember(
                dest => dest.Calories, 
                opt => opt.MapFrom(src => src.Carbohydrates * 4 + src.Proteins * 4 + src.Fats * 9));

            CreateMap<UpdateProductDTO, Product>()
                .ForMember(
                dest => dest.Calories, 
                opt => opt.MapFrom(src => src.Carbohydrates * 4 + src.Proteins * 4 + src.Fats * 9));

            CreateMap<Product, ProductResponse>();
        }
    }
}
