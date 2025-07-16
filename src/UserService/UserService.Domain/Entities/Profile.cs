using UserService.Domain.Enums;


namespace UserService.Domain.Entities;

public class Profile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public double Weight { get; set; }
    public double Height { get; set; }
    public DateOnly Birthday { get; set; }
    public Gender Gender { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
    public int DesiredGlassesOfWater { get; set; }
    public bool ThereIsMealPlan { get; set; } = false;
}
