using AutoMapper;
using FoodService.Application.DTOs.DayResult;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappings
{
    public class DayResultMappingProfile : Profile
    {
        public DayResultMappingProfile()
        {
            CreateMap<CreateDayResultDTO, DayResult>();

            CreateMap<UpdateDayResultDTO, DayResult>();

            CreateMap<DayResult, DayResultDTO>();
        }
    }
}