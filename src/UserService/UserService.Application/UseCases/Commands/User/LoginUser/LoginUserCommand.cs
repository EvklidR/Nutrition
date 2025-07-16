using UserService.Application.DTOs.Responses.User;

namespace UserService.Application.UseCases.Commands
{
    public record LoginUserCommand(string email, string password) : ICommand<AuthenticatedResponse>;
}
