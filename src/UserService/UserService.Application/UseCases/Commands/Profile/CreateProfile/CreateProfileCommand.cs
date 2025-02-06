using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Commands
{
    public record CreateProfileCommand(CreateProfileDTO profileDto) : ICommand<Profile>;
}
