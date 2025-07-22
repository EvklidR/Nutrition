namespace FoodService.Application.DTOs.DayResult.Responses;

public class ShortDayResultResponse
{
    public Guid Id { get; set; }
    public DateOnly Date {  get; set; }
    public double Calories { get; set; }
    public double Proteins { get; set; }
    public double Fats { get; set; }
    public double Carbohydrates { get; set; }
}
