namespace FoodService.Application.DTOs.Statistics;

public class EatenFoodResponse
{
    public string Name { get; set; } = null!;
    public double TotalWeight { get; set; }
    public double TotalCalories { get; set; }
    public double TotalProteins { get; set; }
    public double TotalFats { get; set; }
    public double TotalCarbohydrates { get; set; }
    public bool IsDish {  get; set; }
}
