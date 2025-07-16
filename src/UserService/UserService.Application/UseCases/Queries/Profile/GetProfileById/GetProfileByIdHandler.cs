using AutoMapper;
using UserService.Application.DTOs.Responces.Profile;
using UserService.Contracts.DataAccess.Repositories;
using UserService.Contracts.Exceptions;

namespace UserService.Application.UseCases.Queries;

public class GetProfileByIdHandler : IQueryHandler<GetProfileByIdQuery, ProfileResponse>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IMapper _mapper;

    public GetProfileByIdHandler(IProfileRepository profileRepository, IMapper mapper)
    {
        _profileRepository = profileRepository;
        _mapper = mapper;
    }

    public async Task<ProfileResponse> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepository.GetByIdAsync(request.profileId, cancellationToken);

        if (profile == null)
        {
            throw new NotFound("profile not found");
        }    

        if (profile.UserId != request.userId)
        {
            throw new Unauthorized("Owner isn't valid");
        }

        return _mapper.Map<ProfileResponse>(profile);
    }
}
