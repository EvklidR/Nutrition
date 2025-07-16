namespace UserService.Application.UseCases.Commands
{
    public record ConfirmEmailCommand(Guid UserId, string Code, string? ChangedEmail) : ICommand;
}
