using Microsoft.EntityFrameworkCore;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;
using FoodService.Domain.Entities.Interfaces;

namespace FoodService.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IHasId
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Where(entity => entity.Id == id)
                .FirstOrDefaultAsync();
        }

        public virtual async Task<bool> CheckIfAllEntitiesExistAsync(IEnumerable<Guid> ids)
        {
            var count = await _dbSet
                .Where(entity => ids.Contains(entity.Id))
                .CountAsync();

            return count == ids.Count();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Guid userId)
        {
            return await _dbSet.Where(e => EF.Property<Guid>(e, "UserId") == userId).ToListAsync();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
