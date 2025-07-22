using AutoMapper;
using FoodService.Application.DTOs.DayResult.Requests;
using FoodService.Application.DTOs.DayResult.Responses;
using FoodService.Domain.Entities;

namespace FoodService.Application.Mappers
{
    public class DayResultMappingProfile : Profile
    {
        public DayResultMappingProfile()
        {
            CreateMap<CreateDayResultDTO, DayResult>();

            CreateMap<UpdateDayResultDTO, DayResult>();

            CreateMap<DayResult, DayResultResponse>();
        }
    }
}