namespace FoodService.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> CheckUserByIdAsync(string userId);
        Task<bool> CheckProfileBelonging(string userId, string profileId);
    }
}
