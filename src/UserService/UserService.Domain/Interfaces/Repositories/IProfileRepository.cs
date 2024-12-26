using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repositories
{
    public interface IProfileRepository : IBaseRepository<Profile>
    {
        Task<IEnumerable<Profile>?> GetAllByUserAsync(Guid userId);
    }
}
