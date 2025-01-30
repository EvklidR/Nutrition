namespace FoodService.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> CheckUserByIdAsync(Guid userId);
        Task<bool> CheckProfileBelonging(Guid userId, Guid profileId);
    }
}
