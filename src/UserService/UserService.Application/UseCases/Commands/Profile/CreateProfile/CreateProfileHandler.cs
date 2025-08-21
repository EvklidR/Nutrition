using AutoMapper;
using Newtonsoft.Json;
using UserService.Application.DTOs.Responses.Profile;
using UserService.Contracts.Broker;
using UserService.Contracts.Broker.Enums;
using UserService.Contracts.DataAccess.Repositories;
using UserService.Contracts.Exceptions;
using UserService.Domain.Enums;

namespace UserService.Application.UseCases.Commands;

public class CreateProfileHandler : ICommandHandler<CreateProfileCommand, ProfileResponse>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBrokerService _brokerService;

    private readonly IMapper _mapper;

    public CreateProfileHandler(
        IProfileRepository profileRepository, 
        IUserRepository userRepository, 
        IBrokerService brokerService,
        IMapper mapper)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _brokerService = brokerService;
        _mapper = mapper;
    }

    public async Task<ProfileResponse> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
    {
        var profile = _mapper.Map<Domain.Entities.Profile>(command.ProfileDto);

        var isUserExists = await _userRepository.CheckIfExistsAsync(command.UserId, cancellationToken);

        if (!isUserExists)
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

        var message = JsonConvert.SerializeObject(new
        {
            ProfileId = profile.Id,
            Weight = command.ProfileDto.Weight
        });

        await _brokerService.PublishMessageAsync(message, QueueName.ProfileCreated, exchange: null, cancellationToken: cancellationToken);

        return _mapper.Map<ProfileResponse>(profile);
    }
}
