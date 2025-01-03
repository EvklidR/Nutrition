using UserService.Application.Models;

namespace UserService.Application.UseCases.Commands
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<AuthenticatedResponse>;
}
