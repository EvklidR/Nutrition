namespace MealPlanService.Core.Entities
{
    public class MealPlan
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Purpose { get; set; }
        public string Type { get; set; }
        public List<MealPlanElement> Elements { get; set; } = new List<MealPlanElement>();
    }
}
