namespace FoodService.Application.DTOs
{

    public class CreateOrUpdateEatenFoodDTO
    {
        public Guid FoodId { get; set; }
        public double Weight { get; set; }
    }
}
