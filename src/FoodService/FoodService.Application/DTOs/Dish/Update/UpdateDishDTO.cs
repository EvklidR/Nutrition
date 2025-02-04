using Microsoft.AspNetCore.Http;

namespace FoodService.Application.DTOs.Dish
{
    public class UpdateDishDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int AmountOfPortions { get; set; }
        public IFormFile? Image { get; set; }
        public bool DeleteImageIfNull { get; set; } = false;

        public List<CreateOrUpdateProductOfDishDTO> Ingredients { get; set; } = [];
    }
}
