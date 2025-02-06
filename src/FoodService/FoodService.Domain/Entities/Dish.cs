namespace FoodService.Domain.Entities
{
    public class Dish : Food
    {
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public double WeightOfPortion { get; set; }

        public List<ProductOfDish> Products { get; set; } = new List<ProductOfDish>();
    }
}
