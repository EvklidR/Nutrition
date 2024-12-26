using UserService.Application.DTOs;

namespace UserService.Application.UseCases.Commands
{
    public record UpdateProfileCommand(UpdateProfileDTO profileDto, Guid userId) : ICommand;
}
