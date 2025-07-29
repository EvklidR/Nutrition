using FoodService.Application.Exceptions;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Meal;
using FoodService.Application.Interfaces;

namespace FoodService.Application.UseCases.CommandHandlers.Meal
{
    public class DeleteMealHandler : ICommandHandler<DeleteMealCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public DeleteMealHandler(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task Handle(DeleteMealCommand request, CancellationToken cancellationToken)
        {
            var day = await _unitOfWork.DayResultRepository.GetByIdAsync(request.DayId);

            if (day == null) 
            {
                throw new NotFound("Day not found");
            }

            await _userService.CheckProfileBelongingAsync(
                request.UserId,
                day.ProfileId);

            var meal = day.Meals.FirstOrDefault(m => m.Id == request.MealId);

            if (meal == null) 
            {
                throw new NotFound("Meal not found");
            }

            day.Meals.Remove(meal);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
