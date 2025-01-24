namespace FoodService.Domain.Entities
{
    public class ProductOfDish
    {
        public Guid DishId { get; set; }
        public Guid ProductId { get; set; }
        public double Weight { get; set; }

        public Product Product { get; set; }
    }
}
