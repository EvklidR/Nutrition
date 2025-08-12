using FoodService.Domain.Entities.Interfaces;

namespace FoodService.Domain.Entities;

public class EatenFood : IHasId
{
    public Guid Id { get; set; }
    public Guid MealId { get; set; }
}

public class EatenProduct : EatenFood
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public double Weight { get; set; }
}

public class EatenDish : EatenFood
{
    public Guid DishId { get; set; }
    public Dish Dish { get; set; } = null!;
    public int AmountOfPortions { get; set; }
}
