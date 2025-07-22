namespace PostService.Infrastructure.gRPC.Interfaces
{
    public interface IUserService
    {
        Task<bool> CheckUserExistence(string userId);
    }
}
