using EventsService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using EventsService.Infrastructure.MSSQL;

namespace EventsService.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Add(T newEntity)
        {
            _dbSet.Add(newEntity);
        }

        public void Update(T updatedEntity)
        {
            _dbSet.Update(updatedEntity);
        }

        public void Delete(T entityToDelete)
        {
            _dbSet.Remove(entityToDelete);
        }
    }
}
