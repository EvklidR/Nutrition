using FoodService.Application.DTOs.Meal.Responses;

namespace FoodService.Application.DTOs.DayResult.Responses;

public class DayResultResponse
{
    public Guid Id { get; set; }
    public int GlassesOfWater { get; set; }
    public DateOnly Date { get; set; } 
    public double Weight { get; set; }

    public List<ShortMealResponse> Meals { get; set; } = [];
}
