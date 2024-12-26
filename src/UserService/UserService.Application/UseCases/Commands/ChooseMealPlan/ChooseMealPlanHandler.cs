using UserService.Domain.Interfaces;
using UserService.Application.Exceptions;

namespace UserService.Application.UseCases.Commands
{
    public class ChooseMealPlanHandler : ICommandHandler<ChooseMealPlanCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ChooseMealPlanHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(ChooseMealPlanCommand request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.ProfileRepository.GetByIdAsync(request.profileId);

            if (profile == null)
                throw new NotFound("Profile not found");

            if (request.userId != profile!.UserId)
                throw new Unauthorized("Owner isn't valid");

            profile.MealPlanId = request.mealPlanId;

            _unitOfWork.ProfileRepository.Update(profile);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
