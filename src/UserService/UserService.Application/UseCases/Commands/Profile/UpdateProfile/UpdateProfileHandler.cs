using AutoMapper;
using MediatR;
using UserService.Application.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.UseCases.Commands
{
    public class UpdateProfileHandler : ICommandHandler<UpdateProfileCommand>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public UpdateProfileHandler(IProfileRepository profileRepository, IMapper mapper, IMediator mediator)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByIdAsync(request.profileDto.Id);

            if (profile == null)
                throw new NotFound("Profile not found.");

            if (request.userId != profile!.UserId)
                throw new Unauthorized("Owner isn't valid");

            var existingProfiles = await _profileRepository.GetAllByUserAsync(profile.UserId);

            if (existingProfiles != null)
            {
                foreach (var prof in existingProfiles)
                {
                    if (prof.Name == request.profileDto.Name && prof != profile)
                    {
                        throw new AlreadyExists("Profile with this name in your account already exists");
                    }
                }
            }

            _mapper.Map(request.profileDto, profile);

            await _profileRepository.SaveChangesAsync();
        }
    }
}
