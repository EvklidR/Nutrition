using UserService.Domain.Enums;

namespace UserService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public Role Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
