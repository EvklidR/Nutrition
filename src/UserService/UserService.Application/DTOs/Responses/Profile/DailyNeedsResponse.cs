namespace UserService.Application.DTOs.Responses.Profile;

public class DailyNeedsResponse
{
    public double Calories { get; set; }
    public double Proteins { get; set; }
    public double Fats { get; set; }
    public double Carbohydrates { get; set; }
    public int DesiredGlassesOfWater { get; set; }
}
