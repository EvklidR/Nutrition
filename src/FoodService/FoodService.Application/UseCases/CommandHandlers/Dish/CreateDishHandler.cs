using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Dish;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.Recipe.Responses;

namespace FoodService.Application.UseCases.CommandHandlers.Dish
{
    public class CreateDishHandler : ICommandHandler<CreateDishCommand, CalculatedRecipeResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public CreateDishHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper, IUserService userService, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
            _imageService = imageService;
        }

        public async Task<CalculatedRecipeResponse> Handle(CreateDishCommand request, CancellationToken cancellationToken)
        {
            await _userService.CheckUserByIdAsync(request.UserId);

            var dish = _mapper.Map<Domain.Entities.Dish>(request.CreateDishDTO);
            
            await CalculateNutrientsForDish(dish);

            var filePath = await _imageService.UploadImageAsync(request.CreateDishDTO.Image);

            //dish.ImageUrl = filePath;

            _unitOfWork.DishRepository.Add(dish);

            await _unitOfWork.SaveChangesAsync();

            var dishDTO = _mapper.Map<CalculatedRecipeResponse>(dish);

            return dishDTO;
        }

        private async Task CalculateNutrientsForDish(Domain.Entities.Dish dish)
        {
            double weight = 0;

            //foreach (var ingredient in dish.Ingredients)
            //{
            //    var ingredientBD = await _unitOfWork.ProductRepository.GetByIdAsync(ingredient.ProductId);

            //    if (ingredientBD == null)
            //    {
            //        throw new NotFound("Ingredient not found");
            //    }

            //    dish.Calories += ingredientBD.Calories * ingredient.WeightPerPortion;
            //    dish.Proteins += ingredientBD.Proteins * ingredient.WeightPerPortion;
            //    dish.Fats += ingredientBD.Fats * ingredient.WeightPerPortion;
            //    dish.Carbohydrates += ingredientBD.Carbohydrates * ingredient.WeightPerPortion;
            //    weight += ingredient.WeightPerPortion;
            //}

            dish.Calories = dish.Calories / weight;
            dish.Fats = dish.Fats / weight;
            dish.Proteins = dish.Proteins / weight;
            dish.Carbohydrates = dish.Carbohydrates / weight;
        }
    }
}
