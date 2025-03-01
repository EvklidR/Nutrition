using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.UseCases.Commands
{
    public class DeleteProfileHandler : ICommandHandler<DeleteProfileCommand>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IBrokerService _brokerService;

        public DeleteProfileHandler(
            IProfileRepository profileRepository,
            IBrokerService brokerService)
        {
            _profileRepository = profileRepository;
            _brokerService = brokerService;
        }

        public async Task Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByIdAsync(request.profileId);

            if (profile == null)
            {
                throw new NotFound("Profile not found");
            }

            if (request.userId != profile!.UserId)
            {
                throw new Unauthorized("Owner isn't valid");
            }

            _profileRepository.Delete(profile);

            await _profileRepository.SaveChangesAsync();

            await _brokerService.PublishMessageAsync(profile.Id.ToString(), Enums.QueueName.ProfileDeleted);
        }
    }
}
