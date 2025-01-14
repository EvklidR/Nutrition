using UserService.Domain.Interfaces.Repositories;
using UserService.Application.Exceptions;

namespace UserService.Application.UseCases.Commands
{
    public class ChooseMealPlanHandler : ICommandHandler<ChooseMealPlanCommand>
    {
        private readonly IProfileRepository _profileRepository;
        public ChooseMealPlanHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }
        public async Task Handle(ChooseMealPlanCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByIdAsync(request.profileId);

            if (profile == null)
                throw new NotFound("Profile not found");

            if (request.userId != profile!.UserId)
                throw new Unauthorized("Owner isn't valid");

            profile.ThereIsMealPlan = true;
            _profileRepository.Update(profile);

            await _profileRepository.SaveChangesAsync();
        }
    }
}
