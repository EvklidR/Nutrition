using FoodService.Domain.Entities;

namespace FoodService.Domain.Interfaces.Repositories
{
    public interface IDayResultRepository : IBaseRepository<DayResult>
    {
        Task<IEnumerable<DayResult>?> GetAllByPeriodAsync(int ProfileId, DateOnly dateStart, DateOnly dateEnd);
        Task<DayResult?> GetByDateAsync(int ProfileId, DateOnly date);
    }
}