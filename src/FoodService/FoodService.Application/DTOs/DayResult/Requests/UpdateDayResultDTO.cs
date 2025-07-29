namespace FoodService.Application.DTOs.DayResult.Requests;

public class UpdateDayResultDTO
{
    public Guid Id { get; set; }
    public int GlassesOfWater { get; set; }
    public double Weight { get; set; }
}
