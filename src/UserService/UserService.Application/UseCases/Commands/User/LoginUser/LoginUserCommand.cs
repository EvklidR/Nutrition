using UserService.Application.Models;

namespace UserService.Application.UseCases.Commands
{
    public record LoginUserCommand(string email, string password) : ICommand<AuthenticatedResponse>;
}
