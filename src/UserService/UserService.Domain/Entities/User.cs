using Microsoft.AspNetCore.Identity;

namespace UserService.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}
