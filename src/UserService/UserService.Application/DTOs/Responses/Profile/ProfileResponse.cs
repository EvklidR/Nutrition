using UserService.Domain.Enums;

namespace UserService.Application.DTOs.Responces.Profile;

public class ProfileResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public double Weight { get; set; }
    public double Height { get; set; }
    public DateOnly Birthday { get; set; }
    public Gender Gender { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
}
