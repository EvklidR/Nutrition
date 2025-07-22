namespace FoodService.Application.Interfaces
{
    public interface ICheckUserService
    {
        Task<bool> CheckUserByIdAsync(Guid userId);
        Task<bool> CheckProfileBelonging(Guid userId, Guid profileId);
    }
}
