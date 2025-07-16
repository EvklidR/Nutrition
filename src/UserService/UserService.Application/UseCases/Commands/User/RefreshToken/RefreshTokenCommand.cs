using UserService.Application.DTOs.Responses.User;

namespace UserService.Application.UseCases.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<AuthenticatedResponse>;
