using UserService.Application.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.UseCases.Commands
{
    public class DeleteProfileHandler : ICommandHandler<DeleteProfileCommand>
    {
        private readonly IProfileRepository _profileRepository;

        public DeleteProfileHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByIdAsync(request.profileId);

            if (profile == null)
                throw new NotFound("Profile not found");

            if (request.userId != profile!.UserId)
                throw new Unauthorized("Owner isn't valid");

            _profileRepository.Delete(profile);

            await _profileRepository.SaveChangesAsync();
        }
    }
}
