using MealPlanService.Core.Entities;

namespace MealPlanService.Infrastructure.Repositories.Interfaces
{
    public interface IProfileMealPlanRepository : IBaseRepository<ProfileMealPlan>
    {
        Task<List<ProfileMealPlan>?> GetAllAsync(string profileId);
        Task<ProfileMealPlan?> GetActiveProfilePlan(string profileId);
        Task<IEnumerable<ProfileMealPlan>> GetByMealPlan(string mealPlanId);
    }
}
