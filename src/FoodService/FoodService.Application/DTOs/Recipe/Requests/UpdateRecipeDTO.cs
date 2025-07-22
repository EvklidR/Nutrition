using Microsoft.AspNetCore.Http;

namespace FoodService.Application.DTOs.Recipe.Requests
{
    public class UpdateRecipeDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int AmountOfPortions { get; set; }
        public IFormFile? Image { get; set; }
        public bool DeleteImageIfNull { get; set; } = false;

        public List<CreateOrUpdateProductOfRecipeDTO> Ingredients { get; set; } = [];
    }
}
