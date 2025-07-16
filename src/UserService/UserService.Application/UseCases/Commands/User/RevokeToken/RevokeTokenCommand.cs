namespace UserService.Application.UseCases.Commands;

public record RevokeTokenCommand(Guid? UserId, string RefreshToken) : ICommand;
