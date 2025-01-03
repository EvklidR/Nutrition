namespace UserService.Application.UseCases.Commands
{
    public record ConfirmEmailCommand(Guid userId, string code, string? changedEmail) : ICommand;
}
