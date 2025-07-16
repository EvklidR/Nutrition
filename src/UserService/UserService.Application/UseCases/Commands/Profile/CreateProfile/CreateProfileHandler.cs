using AutoMapper;
using UserService.Application.DTOs.Responces.Profile;
using UserService.Application.Exceptions;
using UserService.Contracts.DataAccess.Repositories;
using UserService.Domain.Enums;
using Profile = UserService.Domain.Entities.Profile;

namespace UserService.Application.UseCases.Commands;

public class CreateProfileHandler : ICommandHandler<CreateProfileCommand, ProfileResponseDto>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CreateProfileHandler(IProfileRepository profileRepository, IUserRepository userRepository, IMapper mapper)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ProfileResponseDto> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
    {
        var profile = _mapper.Map<Profile>(command.ProfileDto);

        var userExists = await _userRepository.CheckIfExistsAsync(command.UserId, cancellationToken);

        if (!userExists)
        {
            throw new Unauthorized("User does not exist");
        }

        profile.UserId = command.UserId;

        var existingProfiles = await _profileRepository.GetAllByUserAsync(profile.UserId, cancellationToken);

        if (existingProfiles != null)
        {
            foreach (var prof in existingProfiles)
            {
                if (prof.Name == profile.Name)
                {
                    throw new AlreadyExists("Profile with this name in your account already exists");
                }
            }
        }

        profile.DesiredGlassesOfWater = profile.Gender == Gender.Female ? 11 : 15;

        await _profileRepository.AddAsync(profile, cancellationToken);

        return _mapper.Map<ProfileResponseDto>(profile);
    }
}
