using FoodService.Domain.Interfaces.Repositories.Models;

namespace FoodService.Domain.Repositories.Models;

public record GetFoodRequestParameters(
    string? Name,
    bool? SortAsc,
    PaginationParameters? PaginationParameters,
    SortingCriteria SortingCriteria = SortingCriteria.Calories);
