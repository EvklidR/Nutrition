using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;
using Microsoft.EntityFrameworkCore;

namespace FoodService.Infrastructure.Repositories;

public class RecipeRepository : BaseRepository<Recipe>, IRecipeRepository
{
    public RecipeRepository(ApplicationDbContext context) : base(context) { }

    public async override Task<Recipe?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Where(recipe => recipe.Id == id)
            .Include(recipe => recipe.Dish)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DoesAnyRecipeContainsProductByIdAsync(Guid id)
    {
        return await _dbSet
            .AnyAsync(recipe => recipe.Ingredients.Any(por => por.ProductId == id));
    }

    public async Task<Recipe?> GetFullByIdAsync(Guid id)
    {
        return await _dbSet
            .Where(r => r.Id == id)
            .Include(r => r.Ingredients)
            .Include(r => r.Dish)
            .FirstOrDefaultAsync();
    }
}
