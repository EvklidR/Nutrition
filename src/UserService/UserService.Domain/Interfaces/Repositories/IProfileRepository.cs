using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repositories
{
    public interface IProfileRepository
    {
        Task<Profile?> GetByIdAsync(Guid id);
        Task<IEnumerable<Profile>?> GetAllByUserAsync(Guid userId);
        void Add(Profile entity);
        void Update(Profile entity);
        void Delete(Profile entity);
        Task SaveChangesAsync();
    }
}
