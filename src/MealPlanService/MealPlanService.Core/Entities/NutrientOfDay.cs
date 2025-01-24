using MealPlanService.Core.Enums;

namespace MealPlanService.Core.Entities
{
    public class NutrientOfDay
    {
        public NutrientType NutrientType { get; set; }
        public CalculationType CalculationType { get; set; }
        public double? Value { get; set; }
    }
}
