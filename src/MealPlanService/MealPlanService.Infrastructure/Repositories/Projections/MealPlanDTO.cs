using MealPlanService.Core.Enums;

namespace MealPlanService.Infrastructure.Projections
{
    public class MealPlanDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MealPlanType Type { get; set; }
    }
}
