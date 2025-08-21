namespace FoodService.Application.DTOs.Dish.Responses;

public class DishesResponse
{
    public List<DishResponse> Dishes { get; set; } = [];
    public long TotalCount { get; set; }
}
