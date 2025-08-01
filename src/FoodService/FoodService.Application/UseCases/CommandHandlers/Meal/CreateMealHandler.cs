using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.Exceptions;
using FoodService.Application.UseCases.Commands.Meal;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.Meal.Responses;

namespace FoodService.Application.UseCases.CommandHandlers.Meal;

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


        var isAllProductsExist = await _unitOfWork.ProductRepository.CheckIfAllEntitiesExistAsync(request.CreateMealDTO.Products.Select(pr => pr.FoodId));

        if (!isAllProductsExist)
        {
            throw new NotFound("Product not found");
        }


        var isAllDishesExist = await _unitOfWork.DishRepository.CheckIfAllEntitiesExistAsync(request.CreateMealDTO.Dishes.Select(d => d.FoodId));

        if (!isAllDishesExist)
        {
            throw new NotFound("Dish not found");
        }

        var meal = _mapper.Map<Domain.Entities.Meal>(request.CreateMealDTO);

        day.Meals.Add(meal);

        await _unitOfWork.SaveChangesAsync();

        var mealDTO = _mapper.Map<FullMealResponse>(meal);

        return mealDTO;
    }
}
