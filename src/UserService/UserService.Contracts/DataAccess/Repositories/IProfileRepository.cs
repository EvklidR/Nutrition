namespace UserService.Contracts.DataAccess.Repositories;

public interface IProfileRepository
{
    Task<Profile?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Profile>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(Profile entity, CancellationToken cancellationToken);
    Task UpdateAsync(Profile entity, CancellationToken cancellationToken);
    Task DeleteAsync(Profile entity, CancellationToken cancellationToken);
}