using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.Exceptions;
using FoodService.Application.UseCases.Commands.Meal;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.Meal.Responses;

namespace FoodService.Application.UseCases.CommandHandlers.Meal
{
    public class CreateMealHandler : ICommandHandler<CreateMealCommand, FullMealResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CreateMealHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<FullMealResponse> Handle(CreateMealCommand request, CancellationToken cancellationToken)
        {
            var day = await _unitOfWork.DayResultRepository.GetByIdAsync(request.CreateMealDTO.DayResultId);

            if (day == null) 
            {
                throw new NotFound("Day not found");
            }

            await _userService.CheckProfileBelongingAsync(
                request.UserId,
                day.ProfileId);

            foreach (var product in request.CreateMealDTO.Products)
            {
                var productDB = await _unitOfWork.ProductRepository.GetByIdAsync(product.FoodId);

                if (productDB == null)
                {
                    throw new NotFound("Product not found");
                }
            }

            foreach (var dish in request.CreateMealDTO.Dishes)
            {
                var dishDB = await _unitOfWork.DishRepository.GetByIdAsync(dish.FoodId);

                if (dishDB == null)
                {
                    throw new NotFound("Dish not found");
                }
            }

            var meal = _mapper.Map<Domain.Entities.Meal>(request.CreateMealDTO);

            day.Meals.Add(meal);

            await _unitOfWork.SaveChangesAsync();

            var mealDTO = _mapper.Map<FullMealResponse>(meal);

            return mealDTO;
        }
    }
}
