using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.Repositories.Models;

namespace MealPlanService.Infrastructure.Repositories.Interfaces
{
    public interface IProfileMealPlanRepository : IBaseRepository<ProfileMealPlan>
    {
        Task<(List<ProfileMealPlan>, long)> GetAllAsync(
            string profileId,
            PaginationParameters? paginationParameters = null,
            PeriodParameters? periodParameters = null);
        Task<ProfileMealPlan?> GetActiveProfilePlan(string profileId);
        Task<IEnumerable<ProfileMealPlan>> GetByMealPlan(string mealPlanId);
    }
}
