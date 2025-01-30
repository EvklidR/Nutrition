using Microsoft.EntityFrameworkCore;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;
using FoodService.Domain.Repositories.Models;
using FoodService.Infrastructure.Extentions;

namespace FoodService.Infrastructure.Repositories
{
    public class DishRepository : BaseRepository<Dish>, IDishRepository
    {
        public DishRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<Dish?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Where(d => d.Id == id)
                .Include(d => d.Ingredients)
                .ThenInclude(iod => iod.Product)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Dish>?> GetAllAsync(Guid userId, GetFoodRequestParameters parameters)
        {
            return await _dbSet
                .Where(d => d.UserId == userId)
                .GetByName(parameters.Name)
                .SortByCriteria(parameters.SortAsc, parameters.SortingCriteria)
                .GetPaginated(parameters.Page, parameters.PageSize)
                .Cast<Dish>()
                .Include(d => d.Ingredients)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Dish>?> GetAllAsync(Guid userId)
        {
            return await _dbSet
                .Where(d => d.UserId == userId)
                .Include(d => d.Ingredients)
                .ToListAsync();
        }
    }
}
