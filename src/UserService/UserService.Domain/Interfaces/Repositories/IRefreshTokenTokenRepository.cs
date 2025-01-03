using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenTokenRepository
    {
        Task<IEnumerable<RefreshToken>?> GetAllByUserAsync(Guid userId);
        void Add(RefreshToken entity);
        void Delete(RefreshToken entity);
        Task SaveChangesAsync();
    }
}
