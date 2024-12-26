using AutoMapper;
using UserService.Application.Exceptions;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces;

namespace UserService.Application.UseCases.Commands
{
    public class CreateProfileHandler : ICommandHandler<CreateProfileCommand, Domain.Entities.Profile>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateProfileHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Domain.Entities.Profile> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = _mapper.Map<Domain.Entities.Profile>(request.profileDto);

            var userExists = await _unitOfWork.UserRepository.GetByIdAsync(profile.UserId);
            if (userExists == null)
            {
                throw new Unauthorized("User does not exist");
            }

            var existingProfiles = await _unitOfWork.ProfileRepository.GetAllByUserAsync(profile.UserId);
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
            _unitOfWork.ProfileRepository.Add(profile);
            await _unitOfWork.SaveChangesAsync();
            return profile;
        }
    }
}
