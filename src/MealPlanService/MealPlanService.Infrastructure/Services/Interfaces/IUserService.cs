namespace MealPlanService.Infrastructure.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> CheckProfileBelonging(string userId, string profileId);
    }
}
