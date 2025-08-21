using UserService.Application.DTOs.Requests.Profile;
using UserService.Application.DTOs.Responses.Profile;

namespace UserService.Application.UseCases.Commands;

public record CreateProfileCommand(CreateProfileDTO ProfileDto, Guid UserId) : ICommand<ProfileResponse>;
