namespace UserService.Application.UseCases.Commands;

public record SendConfirmationToEmailCommand(Guid UserId, string? Url, string Email, bool IsChange = false) : ICommand;