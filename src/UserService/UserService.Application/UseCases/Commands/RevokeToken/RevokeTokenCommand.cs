namespace UserService.Application.UseCases.Commands
{
    public record RevokeTokenCommand(Guid userId) : ICommand;
}
