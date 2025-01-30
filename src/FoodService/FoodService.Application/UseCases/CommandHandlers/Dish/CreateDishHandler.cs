using MediatR;
using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Dish;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.Dish;

namespace FoodService.Application.UseCases.CommandHandlers.Dish
{
    public class CreateDishHandler : ICommandHandler<CreateDishCommand, FullDishDishDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CreateDishHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<FullDishDishDTO> Handle(CreateDishCommand request, CancellationToken cancellationToken)
        {
            var doesUserExist = await _userService.CheckUserByIdAsync(
                request.CreateDishDTO.UserId);

            if (!doesUserExist)
            {
                throw new Forbidden("This user doesn't exist");
            }

            var dish = _mapper.Map<Domain.Entities.Dish>(request.CreateDishDTO);
            
            var ingredients = new List<Domain.Entities.Product>();

            await CalculateNutrientsForDish(dish);

            _unitOfWork.DishRepository.Add(dish);

            await _unitOfWork.SaveChangesAsync();

            var dishDTO = _mapper.Map<FullDishDishDTO>(dish);

            return dishDTO;
        }

        private async Task CalculateNutrientsForDish(Domain.Entities.Dish dish)
        {
            double weight = 0;

            foreach (var ingredient in dish.Ingredients)
            {
                var ingredientBD = await _unitOfWork.ProductRepository.GetByIdAsync(ingredient.ProductId);

                if (ingredientBD == null)
                {
                    throw new NotFound("Ingredient not found");
                }

                dish.Calories += ingredientBD.Calories * ingredient.WeightPerPortion;
                dish.Proteins += ingredientBD.Proteins * ingredient.WeightPerPortion;
                dish.Fats += ingredientBD.Fats * ingredient.WeightPerPortion;
                dish.Carbohydrates += ingredientBD.Carbohydrates * ingredient.WeightPerPortion;
                weight += ingredient.WeightPerPortion;
            }

            dish.Calories = dish.Calories / weight;
            dish.Fats = dish.Fats / weight;
            dish.Proteins = dish.Proteins / weight;
            dish.Carbohydrates = dish.Carbohydrates / weight;
        }
    }
}
