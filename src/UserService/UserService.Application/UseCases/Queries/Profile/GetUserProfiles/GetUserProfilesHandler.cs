using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.UseCases.Queries
{
    public class GetUserProfilesHandler : IQueryHandler<GetUserProfilesQuery, IEnumerable<Profile>?>
    {
        private readonly IProfileRepository _profileRepository;

        public GetUserProfilesHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<IEnumerable<Profile>?> Handle(
            GetUserProfilesQuery request,
            CancellationToken cancellationToken)
        {
            return await _profileRepository.GetAllByUserAsync(request.userId);
        }
    }
}
