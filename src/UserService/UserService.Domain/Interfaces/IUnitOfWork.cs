using UserService.Domain.Interfaces.Repositories;

namespace UserService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IProfileRepository ProfileRepository { get; }
        IConfirmationRepository ConfirmationRepository { get; }
        Task SaveChangesAsync();
    }
}
