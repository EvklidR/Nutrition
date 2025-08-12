namespace FoodService.Application.DTOs.Meal.Responses;

public class MealFoodResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public double ProductCalories { get; set; }
    public double ProductProteins { get; set; }
    public double ProductFats { get; set; }
    public double ProductCarbohydrates { get; set; }
}

public class MealDishResponse : MealFoodResponse
{
    public int AmountOfPortions { get; set; }
    public double WeightOfPortion { get; set; }
}

public class MealProductResponse : MealFoodResponse
{
    public double Weight { get; set; }
}
