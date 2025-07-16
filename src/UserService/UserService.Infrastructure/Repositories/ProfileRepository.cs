using Microsoft.EntityFrameworkCore;
using UserService.Contracts.DataAccess.Repositories;
using UserService.Domain.Entities;
using UserService.Infrastructure.MSSQL;

namespace UserService.Infrastructure.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ApplicationDbContext _context;

    public ProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Profile>?> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Profiles
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Profile?> GetByIdAsync(Guid id,CancellationToken cancellationToken)
    {
        return await _context.Profiles.FindAsync(id, cancellationToken);
    }

    public async Task AddAsync(Profile entity, CancellationToken cancellationToken)
    {
        _context.Profiles.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Profile entity, CancellationToken cancellationToken)
    {
        _context.Profiles.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Profile entity, CancellationToken cancellationToken)
    {
        _context.Profiles.Update(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
