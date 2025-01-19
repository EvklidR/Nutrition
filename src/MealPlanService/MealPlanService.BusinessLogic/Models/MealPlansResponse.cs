using MealPlanService.Infrastructure.Projections;

namespace MealPlanService.BusinessLogic.Models
{
    public class MealPlansResponse
    {
        public List<MealPlanDTO>? MealPlans { get; set; }
        public long TotalCount { get; set; }
    }
}
