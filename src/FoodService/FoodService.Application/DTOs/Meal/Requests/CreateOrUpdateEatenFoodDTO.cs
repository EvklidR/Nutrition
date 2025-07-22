namespace FoodService.Application.DTOs.Meal.Requests
{
    public class CreateOrUpdateEatenFoodDTO
    {
        public Guid FoodId { get; set; }
    }

    public class CreateOrUpdateEatenDishDTO : CreateOrUpdateEatenFoodDTO
    {
        public int AmountOfPortions { get; set; }
    }

    public class CreateOrUpdateEatenProductDTO : CreateOrUpdateEatenFoodDTO
    {
        public double Weight { get; set; }
    }
}
