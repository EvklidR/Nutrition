namespace FoodService.Application.DTOs.Meal
{
    public class CreateOrUpdateEatenFoodDTO
    {
        public Guid FoodId { get; set; }
        public double Weight { get; set; }
    }
}
