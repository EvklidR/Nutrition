using UserService.Application.Models;
using UserService.Application.DTOs;

namespace UserService.Application.UseCases.Commands
{
    public record RegisterUserCommand(CreateUserDTO createUserDto) : ICommand<AuthenticatedResponse>;
}
