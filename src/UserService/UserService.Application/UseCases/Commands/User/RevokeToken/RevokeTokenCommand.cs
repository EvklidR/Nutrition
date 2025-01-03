using System.Globalization;

namespace UserService.Application.UseCases.Commands
{
    public record RevokeTokenCommand(Guid? userId, string refreshToken) : ICommand;
}
