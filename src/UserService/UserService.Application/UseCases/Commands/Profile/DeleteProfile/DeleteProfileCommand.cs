namespace UserService.Application.UseCases.Commands
{
    public record DeleteProfileCommand(Guid profileId, Guid userId) : ICommand;
}
