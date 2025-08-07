using AutoMapper;
using Newtonsoft.Json;
using UserService.Contracts.Broker;
using UserService.Contracts.Broker.Enums;
using UserService.Contracts.DataAccess.Repositories;
using UserService.Contracts.Exceptions;

namespace UserService.Application.UseCases.Commands;

public class UpdateProfileHandler : ICommandHandler<UpdateProfileCommand>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IBrokerService _brokerService;
    private readonly IMapper _mapper;

    public UpdateProfileHandler(
        IProfileRepository profileRepository, 
        IBrokerService brokerService,
        IMapper mapper)
    {
        _profileRepository = profileRepository;
        _brokerService = brokerService;
        _mapper = mapper;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepository.GetByIdAsync(request.ProfileDto.Id, cancellationToken);

        if (profile == null)
        {
            throw new NotFound("Profile not found.");
        }

        if (request.UserId != profile!.UserId)
        {
            throw new Unauthorized("Owner isn't valid");
        }

        if (profile.Name != request.ProfileDto.Name)
        {
            var existingProfiles = await _profileRepository.GetAllByUserAsync(profile.UserId, cancellationToken);

            if (existingProfiles.Any())
            {
                foreach (var prof in existingProfiles)
                {
                    if (prof.Name == request.ProfileDto.Name)
                    {
                        throw new AlreadyExists("Profile with this name in your account already exists");
                    }
                }
            }
        }

        if (profile.Weight != request.ProfileDto.Weight)
        {
            var message = JsonConvert.SerializeObject(new
            {
                ProfileId = profile.Id,
                NewWeight = request.ProfileDto.Weight
            });

            await _brokerService.PublishMessageAsync(message, QueueName.ProfileWeightChanged, exchange: null);
        }

        _mapper.Map(request.ProfileDto, profile);

        await _profileRepository.UpdateAsync(profile, cancellationToken);
    }
}
