using UserService.Application.DTOs.Requests.Profile;

namespace UserService.Application.UseCases.Commands;

public record UpdateProfileCommand(UpdateProfileDTO ProfileDto, Guid UserId) : ICommand;
