using UserService.Contracts.DataAccess.Repositories;
using UserService.Contracts.Exceptions;

namespace UserService.Application.UseCases.Commands.Profile.IncreaseDesiredGlassesOfWater;

public class ChangeDesiredGlassesOfWaterHandler : ICommandHandler<ChangeDesiredGlassesOfWaterCommand>
{
    private readonly IProfileRepository _profileRepository;

    public ChangeDesiredGlassesOfWaterHandler(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task Handle(ChangeDesiredGlassesOfWaterCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepository.GetByIdAsync(request.ProfileId, cancellationToken);

        if (profile == null)
        {
            throw new NotFound("Profile not found");
        }

        if (request.UserId != profile!.UserId)
        {
            throw new Unauthorized("Owner isn't valid");
        }

        profile.DesiredGlassesOfWater = request.DesiredGlassesOfWater;

        await _profileRepository.UpdateAsync(profile, cancellationToken);
    }
}
