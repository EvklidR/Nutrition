using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;

namespace MealPlanService.BusinessLogic.DTOs
{
    public class CreateMealPlanDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public MealPlanType Type { get; set; }

        public List<MealPlanDay> Days { get; set; } = [];
    }
}
