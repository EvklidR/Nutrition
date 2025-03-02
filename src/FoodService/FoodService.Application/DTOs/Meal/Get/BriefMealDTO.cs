namespace FoodService.Application.DTOs.Meal
{
    public class BriefMealDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double TotalCalories { get; set; }
        public double TotalProteins { get; set; }
        public double TotalFats { get; set; }
        public double TotalCarbohydrates { get; set; }
    }
}