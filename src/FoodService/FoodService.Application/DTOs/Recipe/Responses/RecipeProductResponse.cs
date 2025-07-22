using FoodService.Application.DTOs.Product.Responses;

namespace FoodService.Application.DTOs.Recipe.Responses
{
    public class RecipeProductResponse : ProductResponse
    {
        public double Weight { get; set; }
    }
}
