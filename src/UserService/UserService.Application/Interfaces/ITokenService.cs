using UserService.Domain.Entities;
using System.Security.Claims;

namespace UserService.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user);
        Task<string> GenerateRefreshToken(User user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
