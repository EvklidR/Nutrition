using FoodService.Domain.Interfaces.Repositories.Models.enums;

namespace FoodService.Domain.Repositories.Models
{
    public record GetFoodRequestParameters(
        string? Name,
        int? Page, 
        int? PageSize,
        bool? SortAsc,
        SortingCriteria? SortingCriteria = SortingCriteria.Calories);
}
