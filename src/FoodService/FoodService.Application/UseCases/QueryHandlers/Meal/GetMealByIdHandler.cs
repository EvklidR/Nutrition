using AutoMapper;
using FoodService.Application.DTOs.Meal.Responses;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Queries.Meal;
using FoodService.Domain.Interfaces;

namespace FoodService.Application.UseCases.QueryHandlers.Meal.GetMealById
{
    public class GetMealByIdHandler : IQueryHandler<GetMealByIdQuery, FullMealResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICheckUserService _userService;

        public GetMealByIdHandler(IMapper mapper, IUnitOfWork unitOfWork, ICheckUserService userService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<FullMealResponse> Handle(GetMealByIdQuery request, CancellationToken cancellationToken)
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

            var mealDTO = _mapper.Map<FullMealResponse>(meal);

            return mealDTO;
        }
    }
}
