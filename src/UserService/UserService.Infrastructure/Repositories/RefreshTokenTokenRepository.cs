using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;
using UserService.Infrastructure.MSSQL;

namespace UserService.Infrastructure.Repositories
{
    public class RefreshTokenTokenRepository : IRefreshTokenTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public RefreshTokenTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(RefreshToken entity)
        {
            _context.RefreshTokens.Add(entity);
        }

        public void Delete(RefreshToken entity)
        {
            _context.RefreshTokens.Remove(entity);
        }

        public async Task<IEnumerable<RefreshToken>?> GetAllByUserAsync(Guid userId)
        {
            return await _context.RefreshTokens
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
