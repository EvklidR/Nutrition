using UserService.Domain.Entities;
using System.Security.Claims;

namespace UserService.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
