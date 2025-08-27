using AutoMapper;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.Core.Entities;

namespace MealPlanService.BusinessLogic.Mappers
{
    public class MealPlanProfile : Profile
    {
        public MealPlanProfile()
        {
            CreateMap<CreateMealPlanDTO, MealPlan>();
        }
    }
}
