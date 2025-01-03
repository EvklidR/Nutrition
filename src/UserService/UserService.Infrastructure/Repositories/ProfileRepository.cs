using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;
using UserService.Infrastructure.MSSQL;

namespace UserService.Infrastructure.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _context;
        public ProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Profile>?> GetAllByUserAsync(Guid userId)
        {
            return await _context.Profiles
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Profile?> GetByIdAsync(Guid id)
        {
            return await _context.Profiles.FindAsync(id);
        }

        public void Add(Profile entity)
        {
            _context.Profiles.Add(entity);
        }

        public void Delete(Profile entity)
        {
            _context.Profiles.Remove(entity);
        }

        public void Update(Profile entity)
        {
            _context.Profiles.Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
