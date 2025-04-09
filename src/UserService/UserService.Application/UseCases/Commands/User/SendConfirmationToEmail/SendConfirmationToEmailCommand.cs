namespace UserService.Application.UseCases.Commands
{
    public record SendConfirmationToEmailCommand(Guid userId, string? url, string email, bool isChange = false) : ICommand;
}