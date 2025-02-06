using UserService.Domain.Enums;

namespace UserService.Application.DTOs
{
    public class CreateProfileDTO
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public DateOnly Birthday { get; set; }
        public Gender Gender { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
    }
}
