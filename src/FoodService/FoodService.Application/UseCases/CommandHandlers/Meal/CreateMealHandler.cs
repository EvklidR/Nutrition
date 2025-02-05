using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.Exceptions;
using FoodService.Application.UseCases.Commands.Meal;
using FoodService.Application.DTOs.Meal;
using FoodService.Application.Interfaces;

namespace FoodService.Application.UseCases.CommandHandlers.Meal
{
    public class CreateMealHandler : ICommandHandler<CreateMealCommand, FullMealDTO>
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

        public async Task<FullMealDTO> Handle(CreateMealCommand request, CancellationToken cancellationToken)
        {
            var day = await _unitOfWork.DayResultRepository.GetByIdAsync(request.CreateMealDTO.DayId);

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

            var mealDTO = _mapper.Map<FullMealDTO>(meal);

            return mealDTO;
        }
    }
}
