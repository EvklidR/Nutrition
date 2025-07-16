namespace UserService.Contracts.DataAccess.Repositories;

public interface IUserRepository
{
    Task<bool> CheckIfExistsAsync(Guid id, CancellationToken cancellationToken);
}