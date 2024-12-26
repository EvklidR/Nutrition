namespace UserService.Application.UseCases.Commands
{
    public record SendConfirmationCommand(string email) : ICommand;
}
