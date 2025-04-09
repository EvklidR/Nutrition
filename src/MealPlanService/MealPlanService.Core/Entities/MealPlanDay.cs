namespace MealPlanService.Core.Entities
{
    public class MealPlanDay
    {
        public int DayNumber { get; set; }
        public double CaloriePercentage { get; set; } = 1;

        public List<Recommendation> Recommendations { get; set; } = [];
        public List<NutrientOfDay> NutrientsOfDay { get; set; } = [];
    }
}
