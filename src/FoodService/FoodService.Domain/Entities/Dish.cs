using FoodService.Domain.Entities;

namespace FoodService.Domain.Entities
{
    public class Dish : Food
    {
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public double WeightOfPortion { get; set; }

        public List<ProductOfDish> Ingredients { get; set; } = new List<ProductOfDish>();
    }
}
