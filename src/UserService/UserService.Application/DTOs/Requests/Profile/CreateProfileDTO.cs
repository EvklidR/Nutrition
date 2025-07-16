using UserService.Domain.Enums;

namespace UserService.Application.DTOs.Requests.Profile;

public class CreateProfileDTO
{
    public string Name { get; set; } = null!;
    public double Weight { get; set; }
    public double Height { get; set; }
    public DateOnly Birthday { get; set; }
    public Gender Gender { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
}
