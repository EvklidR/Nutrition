using UserService.Application.Models;

namespace UserService.Application.UseCases.Commands
{
    public record RefreshTokenCommand(string accessToken, string refreshToken) : ICommand<AuthenticatedResponse>;
}
