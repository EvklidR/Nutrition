using AutoMapper;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.Core.Entities;

namespace MealPlanService.BusinessLogic.Mappers
{
    public class MealPlanProfile : Profile
    {
        public MealPlanProfile()
        {
            // MealPlanDTO -> MealPlan
            CreateMap<CreateMealPlanDTO, MealPlan>();
        }
    }
}
