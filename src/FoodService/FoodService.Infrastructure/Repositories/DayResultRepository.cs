using Microsoft.EntityFrameworkCore;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;
using FoodService.Domain.Interfaces.Repositories.Models;
using FoodService.Infrastructure.IQueriableExtentions;

namespace FoodService.Infrastructure.Repositories
{
    public class DayResultRepository : BaseRepository<DayResult>, IDayResultRepository
    {
        public DayResultRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<DayResult?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Where(d => d.Id == id)
                .IncludeFood()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DayResult>> GetAllByParametersAsync(
            Guid ProfileId, 
            PaginatedParameters? paginatedParameters, 
            PeriodParameters? periodParameters)
        {
            return await _dbSet
                .GetByPeriod(periodParameters)
                .GetPaginated(paginatedParameters)
                .Where(dr => dr.ProfileId == ProfileId)
                .IncludeFood()
                .ToListAsync();
        }

        public async Task<DayResult?> GetByDateAsync(Guid ProfileId, DateOnly date)
        {
            return await _dbSet
                .Where(dr => dr.Date == date)
                .Where(dr => dr.ProfileId == ProfileId)
                .IncludeFood()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DoesAnyDayResultContainsFoodByIdAsync(Guid id, bool aboutProduct)
        {
            if (aboutProduct)
            {
                return await _dbSet.AnyAsync(
                        dr => dr.Meals.Any(
                                meal => meal.Products.Any(
                                        pr => pr.Id == id
                                    )
                            )
                    );
            }
            else
            {
                return await _dbSet.AnyAsync(
                        dr => dr.Meals.Any(
                                meal => meal.Dishes.Any(
                                        dish => dish.Id == id
                                    )
                            )
                    );
            }
        }
    }
}
