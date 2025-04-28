using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;
using MealPlanService.Infrastructure.Projections;

namespace MealPlanService.Infrastructure.Repositories.Interfaces
{
    public interface IMealPlanRepository : IBaseRepository<MealPlan>
    {
        Task<(List<MealPlanDTO>?, long)> GetAllAsync(MealPlanType? type, int? page, int? size);
        Task<List<MealPlanDTO>> GetManyByIdsAsync(List<string> ids);
    }
}
