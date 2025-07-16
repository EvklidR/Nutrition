using UserService.Domain.Entities;

namespace UserService.Contracts.DataAccess.Repositories;

public interface IRefreshTokenTokenRepository
{
    Task<IEnumerable<RefreshToken>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(RefreshToken entity, CancellationToken cancellationToken);
    Task DeleteAsync(RefreshToken entity, CancellationToken cancellationToken);
}