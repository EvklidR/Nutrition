namespace UserService.Application.UseCases.Commands;

public record DeleteProfileCommand(Guid ProfileId, Guid UserId) : ICommand;
