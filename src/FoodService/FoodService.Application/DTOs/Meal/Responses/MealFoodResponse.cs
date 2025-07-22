namespace FoodService.Application.DTOs.Meal.Responses;

public class MealFoodResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public double TotalProductCalories { get; set; }
    public double TotalProductProteins { get; set; }
    public double TotalProductFats { get; set; }
    public double TotalProductCarbohydrates { get; set; }
}

public class MealDishResponse : MealFoodResponse
{
    public int AmountOfPortions { get; set; }
}

public class MealProductResponse : MealFoodResponse
{
    public double Weight { get; set; }
}
