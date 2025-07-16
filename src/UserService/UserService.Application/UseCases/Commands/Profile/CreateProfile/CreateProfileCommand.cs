using UserService.Application.DTOs.Requests.Profile;
using UserService.Application.DTOs.Responces.Profile;

namespace UserService.Application.UseCases.Commands;

public record CreateProfileCommand(CreateProfileDTO ProfileDto, Guid UserId) : ICommand<ProfileResponseDto>;
