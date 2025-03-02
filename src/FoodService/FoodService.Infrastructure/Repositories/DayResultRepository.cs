using Microsoft.EntityFrameworkCore;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;

namespace FoodService.Infrastructure.Repositories
{
    public class DayResultRepository : BaseRepository<DayResult>, IDayResultRepository
    {
        public DayResultRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<DayResult?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Where(d => d.Id == id)
                .Include(dr => dr.Meals)
                    .ThenInclude(m => m.Dishes)
                        .ThenInclude(f => f.Food)
                .Include(d => d.Meals)
                    .ThenInclude(m => m.Products)
                        .ThenInclude(p => p.Food)
                .FirstOrDefaultAsync();
        }

        public override async Task<IEnumerable<DayResult>?> GetAllAsync(Guid profileId) 
        {
            return await _dbSet
                .Where(e => e.ProfileId == profileId)
                .Include(dr => dr.Meals)
                    .ThenInclude(m => m.Dishes)
                .Include(d => d.Meals)
                    .ThenInclude(m => m.Products)
                .ToListAsync();
        }

        public async Task<IEnumerable<DayResult>?> GetAllByPeriodAsync(Guid ProfileId, DateOnly dateStart, DateOnly dateEnd)
        {
            return await _dbSet
                .Where(dr => dr.Date >= dateStart && dr.Date <= dateEnd)
                .Where(dr => dr.ProfileId == ProfileId)
                .Include(dr => dr.Meals)
                    .ThenInclude(m => m.Dishes)
                        .ThenInclude(f => f.Food)
                .Include(d => d.Meals)
                    .ThenInclude(m => m.Products)
                        .ThenInclude(p => p.Food)
                .ToListAsync();
        }

        public async Task<DayResult?> GetByDateAsync(Guid ProfileId, DateOnly date)
        {
            return await _dbSet
                .Where(dr => dr.Date == date)
                .Where(dr => dr.ProfileId == ProfileId)
                .Include(dr => dr.Meals)
                    .ThenInclude(m => m.Dishes)
                        .ThenInclude(f => f.Food)
                .Include(d => d.Meals)
                    .ThenInclude(m => m.Products)
                        .ThenInclude(p => p.Food)
                .FirstOrDefaultAsync();
        }
    }
}
