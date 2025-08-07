using Microsoft.EntityFrameworkCore;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;
using FoodService.Domain.Repositories.Models;
using FoodService.Infrastructure.IQueriableExtentions;

namespace FoodService.Infrastructure.Repositories
{
    public class DishRepository : BaseRepository<Dish>, IDishRepository
    {
        public DishRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Dish?> GetByIdWithRecipeAsync(Guid id)
        {
            return await _dbSet
                .Where(d => d.Id == id)
                .Include(d => d.Recipe)
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Dish>?, long)> GetAllAsync(Guid userId, GetFoodRequestParameters parameters)
        {
            var query = _dbSet
                .Where(d => d.UserId == userId)
                .GetByName(parameters.Name)
                .SortByCriteria(parameters.SortAsc, parameters.SortingCriteria);

            long totalRecords = await query.CountAsync();

            var dishes = await query
                .GetPaginated(parameters.PaginationParameters)
                .Cast<Dish>()
                .ToListAsync();

            return (dishes, totalRecords);
        }


        public override async Task<IEnumerable<Dish>?> GetAllAsync(Guid userId)
        {
            return await _dbSet
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }
    }
}
