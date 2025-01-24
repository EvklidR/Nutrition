using Microsoft.AspNetCore.Http;

namespace FoodService.Application.DTOs
{
    public class CreateDishDTO
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int AmountOfPortions { get; set; }
        public List<IFormFile> Images { get; set; } = [];

        public List<ProductOfDishDTO> Ingredients { get; set; } = [];
    }
}
