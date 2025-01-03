using UserService.Domain.Enums;

namespace UserService.Application.DTOs
{
    public class UpdateProfileDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public ActivityLevel? ActivityLevel { get; set; }
        public int? DesiredGlassesOfWater { get; set; }
    }
}
