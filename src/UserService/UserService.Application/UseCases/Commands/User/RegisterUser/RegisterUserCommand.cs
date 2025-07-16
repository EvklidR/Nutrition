namespace UserService.Application.UseCases.Commands;

public record RegisterUserCommand(string Email, string Password, string? Url) : ICommand;
