using Microsoft.EntityFrameworkCore;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;

namespace FoodService.Infrastructure.Repositories
{
    public class DishRepository : BaseRepository<Dish>, IDishRepository
    {
        public DishRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Dish?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Ingredients)
                .ThenInclude(iod => iod.Ingredient)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
