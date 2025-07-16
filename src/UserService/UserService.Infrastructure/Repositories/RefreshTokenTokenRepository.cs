using Microsoft.EntityFrameworkCore;
using UserService.Contracts.DataAccess.Repositories;
using UserService.Domain.Entities;
using UserService.Infrastructure.MSSQL;

namespace UserService.Infrastructure.Repositories;

public class RefreshTokenTokenRepository : IRefreshTokenTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken entity, CancellationToken cancellationToken)
    {
        _context.RefreshTokens.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(RefreshToken entity, CancellationToken cancellationToken)
    {
        _context.RefreshTokens.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<RefreshToken>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}
