using UserService.Application.Models;

namespace UserService.Application.Interfaces
{
    public interface IMealPlanService
    {
        Task<DailyNeedsResponse> GetDailyNeedsByMealPlanAsync(
            Guid userId,
            double bodyWeight,
            double dailyKcal);
    }
}
