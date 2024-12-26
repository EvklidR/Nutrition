using UserService.Domain.Interfaces;
using UserService.Application.Exceptions;

namespace UserService.Application.UseCases.Commands
{
    public class RevokeMealPlanHandler : ICommandHandler<RevokeMealPlanCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RevokeMealPlanHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(RevokeMealPlanCommand request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.ProfileRepository.GetByIdAsync(request.profileId);
            if (profile == null)
                throw new NotFound("Profile not found");

            if (request.userId != profile!.UserId)
                throw new Unauthorized("Owner isn't valid");

            profile.MealPlanId = null;

            _unitOfWork.ProfileRepository.Update(profile);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
