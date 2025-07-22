namespace FoodService.Application.DTOs.Recipe.Requests
{
    public class CreateOrUpdateProductOfRecipeDTO
    {
        public Guid ProductId { get; set; }
        public double Weight { get; set; }
    }
}
