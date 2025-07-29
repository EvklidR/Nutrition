namespace FoodService.Application.Interfaces
{
    public interface IUserService
    {
        Task CheckUserByIdAsync(Guid userId);
        Task CheckProfileBelongingAsync(Guid userId, Guid profileId);
        Task<double> GetProfileWeightAsync(Guid profileId);
    }
}
