namespace FoodService.Domain.Entities;

public class EatenFood
{
    public Guid Id { get; set; }
    public Guid FoodId { get; set; }
    public Guid MealId { get; set; }

    public Food Food { get; set; } = null!;
}

public class EatenProduct : EatenFood
{
    public double Weight { get; set; }
}

public class EatenDish : EatenFood
{
    public int AmountOfPortions { get; set; }
}
