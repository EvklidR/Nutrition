using Microsoft.EntityFrameworkCore;
using UserService.Contracts.DataAccess.Repositories;
using UserService.Infrastructure.MSSQL;

namespace UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CheckIfExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
    }
}
