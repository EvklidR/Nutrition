using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories.Models;

namespace FoodService.Domain.Interfaces.Repositories;

public interface IDayResultRepository : IBaseRepository<DayResult>
{
    Task<IEnumerable<DayResult>> GetAllByParametersAsync(
        Guid profileId, 
        PaginatedParameters? paginatedParameters, 
        PeriodParameters? periodParameters);
    Task<DayResult?> GetByDateAsync(Guid profileId, DateOnly date);
    Task<bool> DoesAnyDayResultContainsFoodByIdAsync(Guid id, bool aboutProduct);
}