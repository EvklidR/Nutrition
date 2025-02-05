using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.Exceptions;
using FoodService.Application.UseCases.Commands.Meal;
using FoodService.Application.Interfaces;

namespace FoodService.Application.UseCases.CommandHandlers.Meal
{
    public class UpdateMealHandler : ICommandHandler<UpdateMealCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UpdateMealHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task Handle(UpdateMealCommand request, CancellationToken cancellationToken)
        {
            var day = await _unitOfWork.DayResultRepository.GetByIdAsync(request.UpdateMealDTO.DayId);

            if (day == null)
            {
                throw new NotFound("Day not found");
            }

            var doesProfileBelongUser = await _userService.CheckProfileBelonging(
                request.UserId,
                day.ProfileId);

            if (!doesProfileBelongUser)
            {
                throw new Forbidden("You dont have access to this meal");
            }

            var meal = day.Meals.FirstOrDefault(m => m.Id == request.UpdateMealDTO.Id);

            if (meal == null)
            {
                throw new NotFound("Meal not found");
            }

            _mapper.Map(request.UpdateMealDTO, meal);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
