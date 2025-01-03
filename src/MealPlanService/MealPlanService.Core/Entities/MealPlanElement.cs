namespace MealPlanService.Core.Entities
{
    public class MealPlanElement
    {
        public Guid Id { get; set; }
        public Guid MealPlanId { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public object? InnerValue { get; set; }
    }
}
