namespace FoodService.Domain.Entities;

public class Dish : Food
{
    public double WeightOfPortion { get; set; }
    public Guid RecipeId { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
