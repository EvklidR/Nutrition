namespace UserService.Application.UseCases.Commands
{
    public record RegisterUserCommand(string email, string password, string? url) : ICommand;
}
