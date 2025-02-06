using UserService.Application.Exceptions;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.UseCases.Queries
{
    public class GetProfileByIdHandler : IQueryHandler<GetProfileByIdQuery, Profile>
    {
        private readonly IProfileRepository _profileRepository;
        public GetProfileByIdHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<Profile> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByIdAsync(request.profileId);

            if (profile == null)
            {
                throw new NotFound("profile not found");
            }    

            if (profile.UserId != request.userId)
            {
                throw new Unauthorized("Owner isn't valid");
            }

            return profile;
        }
    }
}
