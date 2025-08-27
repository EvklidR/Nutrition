using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories.Models;

namespace FoodService.Domain.Interfaces.Repositories;

public interface IDayResultRepository : IBaseRepository<DayResult>
{
    Task<(IEnumerable<DayResult>, long)> GetAllByParametersAsync(
        Guid profileId, 
        PaginationParameters? paginationParameters = null, 
        PeriodParameters? periodParameters = null);
    Task<DayResult?> GetByDateAsync(Guid profileId, DateOnly date);
    Task<bool> DoesAnyDayResultContainsFoodByIdAsync(Guid id, bool aboutProduct);
}