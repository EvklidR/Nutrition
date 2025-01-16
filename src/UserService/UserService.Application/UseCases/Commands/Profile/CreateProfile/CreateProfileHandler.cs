using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Exceptions;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.UseCases.Commands
{
    public class CreateProfileHandler : ICommandHandler<CreateProfileCommand, Domain.Entities.Profile>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public CreateProfileHandler(IProfileRepository profileRepository, UserManager<User> userManager, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Domain.Entities.Profile> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = _mapper.Map<Domain.Entities.Profile>(request.profileDto);

            var userExists = await _userManager.FindByIdAsync(profile.UserId.ToString());
            if (userExists == null)
            {
                throw new Unauthorized("User does not exist");
            }

            var existingProfiles = await _profileRepository.GetAllByUserAsync(profile.UserId);
            if (existingProfiles != null)
            {
                foreach (var prof in existingProfiles)
                {
                    if (prof.Name == profile.Name)
                    {
                        throw new AlreadyExists("Profile with this name in your account already exists");
                    }
                }
            }
            profile.DesiredGlassesOfWater = profile.Gender == Gender.Female ? 11 : 15;
            _profileRepository.Add(profile);

            await _profileRepository.SaveChangesAsync();

            return profile;
        }
    }
}
