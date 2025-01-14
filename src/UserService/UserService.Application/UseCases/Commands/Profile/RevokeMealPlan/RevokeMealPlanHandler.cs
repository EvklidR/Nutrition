using UserService.Domain.Interfaces.Repositories;
using UserService.Application.Exceptions;

namespace UserService.Application.UseCases.Commands
{
    public class RevokeMealPlanHandler : ICommandHandler<RevokeMealPlanCommand>
    {
        private readonly IProfileRepository _profileRepository;
        public RevokeMealPlanHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }
        public async Task Handle(RevokeMealPlanCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByIdAsync(request.profileId);

            if (profile == null)
                throw new NotFound("Profile not found");

            if (request.userId != profile!.UserId)
                throw new Unauthorized("Owner isn't valid");

            profile.ThereIsMealPlan = false;
            _profileRepository.Update(profile);

            await _profileRepository.SaveChangesAsync();
        }
    }
}
