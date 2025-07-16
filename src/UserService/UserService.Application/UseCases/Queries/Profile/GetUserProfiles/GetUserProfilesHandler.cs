using AutoMapper;
using UserService.Application.DTOs.Responses.Profile;
using UserService.Contracts.DataAccess.Repositories;

namespace UserService.Application.UseCases.Queries
{
    public class GetUserProfilesHandler : IQueryHandler<GetUserProfilesQuery, IEnumerable<ShortProfileResponse>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public GetUserProfilesHandler(IProfileRepository profileRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShortProfileResponse>> Handle(
            GetUserProfilesQuery request,
            CancellationToken cancellationToken)
        {
            var profiles = await _profileRepository.GetAllByUserAsync(request.UserId, cancellationToken);

            return _mapper.Map<List<ShortProfileResponse>>(profiles);
        }
    }
}
