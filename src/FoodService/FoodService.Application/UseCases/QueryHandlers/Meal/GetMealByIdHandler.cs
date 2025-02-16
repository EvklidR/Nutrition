using AutoMapper;
using FoodService.Application.DTOs.Meal;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Queries.Meal;
using FoodService.Domain.Interfaces;

namespace FoodService.Application.UseCases.QueryHandlers.Meal.GetMealById
{
    public class GetMealByIdHandler : IQueryHandler<GetMealByIdQuery, FullMealDTO>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public GetMealByIdHandler(IMapper mapper, IUnitOfWork unitOfWork, IUserService userService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<FullMealDTO> Handle(GetMealByIdQuery request, CancellationToken cancellationToken)
        {
            var day = await _unitOfWork.DayResultRepository.GetByIdAsync(request.DayId);

            if (day == null)
            {
                throw new NotFound("Day not found");
            }

            var isProfileBelongUser = await _userService.CheckProfileBelonging(
                request.UserId,
                day.ProfileId);

            if (!isProfileBelongUser)
            {
                throw new Forbidden("You dont have access to this meal");
            }

            var meal = day.Meals.FirstOrDefault(m => m.Id == request.MealId);

            if (meal == null)
            {
                throw new NotFound("Meal not found");
            }

            var mealDTO = _mapper.Map<FullMealDTO>(meal);

            return mealDTO;
        }
    }
}
