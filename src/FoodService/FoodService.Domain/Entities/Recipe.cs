using FoodService.Domain.Entities.Interfaces;

namespace FoodService.Domain.Entities;

public class Recipe : IHasId
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int AmountOfPortions { get; set; }
    public Guid DishId { get; set; }

    public Dish Dish { get; set; } = null!;
    public List<ProductOfRecipe> Ingredients { get; set; } = new List<ProductOfRecipe>();
}
