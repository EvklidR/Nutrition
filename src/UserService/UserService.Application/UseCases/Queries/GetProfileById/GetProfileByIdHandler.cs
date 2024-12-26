using UserService.Application.Exceptions;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.UseCases.Queries
{
    public class GetProfileByIdHandler : IQueryHandler<GetProfileByIdQuery, Profile>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetProfileByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Profile> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.ProfileRepository.GetByIdAsync(request.profileId);
            if (profile == null)
                throw new NotFound("profile not found");

            if (profile.UserId != request.userId)
                throw new Unauthorized("Owner isn't valid");
            return profile;
        }
    }
}
