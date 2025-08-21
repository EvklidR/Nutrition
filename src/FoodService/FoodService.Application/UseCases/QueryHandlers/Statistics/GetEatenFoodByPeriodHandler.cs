using AutoMapper;
using FoodService.Application.DTOs.Statistics;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Queries.Statistics;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces;

namespace FoodService.Application.UseCases.QueryHandlers.Statistics;

public class GetEatenFoodByPeriodHandler : IQueryHandler<GetEatenFoodByPeriodQuery, IEnumerable<EatenFoodResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public GetEatenFoodByPeriodHandler(
        IUnitOfWork unitOfWork,
        IUserService userService,
        IMapper mapper
        )
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EatenFoodResponse>> Handle(GetEatenFoodByPeriodQuery request, CancellationToken cancellationToken)
    {
        await _userService.CheckProfileBelongingAsync(request.UserId, request.ProfileId);

        var dayResults = await _unitOfWork.DayResultRepository.GetAllByParametersAsync(request.ProfileId, paginatedParameters: null, request.PeriodParameters);

        var meals = dayResults.SelectMany(dayResult => dayResult.Meals);

        var eatenProducts = meals.SelectMany(meal => meal.Products);
        var eatenDishes = meals.SelectMany(meal => meal.Dishes);

        var distinctProductIds = eatenProducts.Select(p => p.ProductId).Distinct();
        var distinctDishesIds = eatenDishes.Select(d => d.DishId).Distinct();

        var response = new List<EatenFoodResponse>();

        CalculateProducts(distinctProductIds, eatenProducts, response);
        CalculateDishes(distinctDishesIds, eatenDishes, response);

        return response;
    }

    private void CalculateProducts(IEnumerable<Guid> distinctProductIds, IEnumerable<EatenProduct> eatenProducts, List<EatenFoodResponse> response)
    {
        foreach (var id in distinctProductIds)
        {
            var products = eatenProducts.Where(ed => ed.ProductId == id).ToList();

            response.Add(new EatenFoodResponse
            {
                Name = products[0].Product.Name,
                TotalWeight = products.Select(ep => ep.Weight).Sum(),
                TotalCalories = products.Select(ep => ep.Weight * ep.Product.Calories / 100).Sum(),
                TotalProteins = products.Select(ep => ep.Weight * ep.Product.Proteins / 100).Sum(),
                TotalFats = products.Select(ep => ep.Weight * ep.Product.Fats / 100).Sum(),
                TotalCarbohydrates = products.Select(ep => ep.Weight * ep.Product.Carbohydrates / 100).Sum(),
                IsDish = false
            });
        }
    }

    private void CalculateDishes(IEnumerable<Guid> distinctDishesIds, IEnumerable<EatenDish> eatenDishes, List<EatenFoodResponse> response)
    {
        foreach ( var id in distinctDishesIds )
        {
            var dishes = eatenDishes.Where(ed => ed.DishId == id).ToList();

            response.Add(new EatenFoodResponse
            {
                Name = dishes[0].Dish.Name,
                TotalWeight = dishes.Select(ed => ed.AmountOfPortions * ed.Dish.WeightOfPortion).Sum(),
                TotalCalories = dishes.Select(ed => ed.AmountOfPortions * ed.Dish.WeightOfPortion * ed.Dish.Calories / 100).Sum(),
                TotalProteins = dishes.Select(ed => ed.AmountOfPortions * ed.Dish.WeightOfPortion * ed.Dish.Proteins / 100).Sum(),
                TotalFats = dishes.Select(ed => ed.AmountOfPortions * ed.Dish.WeightOfPortion * ed.Dish.Fats / 100).Sum(),
                TotalCarbohydrates = dishes.Select(ed => ed.AmountOfPortions * ed.Dish.WeightOfPortion * ed.Dish.Carbohydrates / 100).Sum(),
                IsDish = true
            });
        }
    }
}
