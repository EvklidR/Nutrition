namespace FoodService.Application.DTOs.Recipe.Responses
{
    public class RecipeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int AmountOfPortions { get; set; }

        public List<RecipeProductResponse> Ingredients { get; set; } = [];
    }
}
