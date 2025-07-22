using Microsoft.AspNetCore.Http;

namespace FoodService.Application.DTOs.Recipe.Requests
{
    public class CreateRecipeDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int AmountOfPortions { get; set; }
        public IFormFile? Image { get; set; }

        public List<CreateOrUpdateProductOfRecipeDTO> Ingredients { get; set; } = [];
    }
}
