namespace FoodService.Application.DTOs.Meal
{
    public class MealFoodDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double TotalProductCalories { get; set; }
        public double TotalProductProteins { get; set; }
        public double TotalProductFats { get; set; }
        public double TotalProductCarbohydrates { get; set; }
        public double Weight { get; set; }
    }
}
