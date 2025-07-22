namespace FoodService.Domain.Entities
{
    public class ProductOfRecipe
    {
        public Guid RecipeId { get; set; }
        public Guid ProductId { get; set; }
        public double WeightInRecipe { get; set; }

        public Product Product { get; set; } = null!;
    }
}
