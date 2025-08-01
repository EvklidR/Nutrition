using FoodService.Domain.Entities.Interfaces;

namespace FoodService.Domain.Entities;

public class Dish : Food
{
    public Guid RecipeId { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
