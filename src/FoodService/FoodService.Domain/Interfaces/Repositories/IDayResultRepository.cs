using FoodService.Domain.Entities;

namespace FoodService.Domain.Interfaces.Repositories
{
    public interface IDayResultRepository : IBaseRepository<DayResult>
    {
        Task<IEnumerable<DayResult>?> GetAllByPeriodAsync(Guid ProfileId, DateOnly dateStart, DateOnly dateEnd);
        Task<DayResult?> GetByDateAsync(Guid ProfileId, DateOnly date);
    }
}