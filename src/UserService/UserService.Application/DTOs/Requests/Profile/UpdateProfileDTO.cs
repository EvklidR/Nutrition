using UserService.Domain.Enums;

namespace UserService.Application.DTOs.Requests.Profile;

public class UpdateProfileDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public double Weight { get; set; }
    public double Height { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
}
