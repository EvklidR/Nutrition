namespace FoodService.Domain.Entities
{
    public class Dish : Food
    {
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public List<ProductOfDish> Ingredients { get; set; } = [];
    }
}
