using FoodService.Domain.Entities;

namespace FoodService.Domain.Interfaces.Repositories;

public interface IRecipeRepository : IBaseRepository<Recipe>
{
    Task<Recipe?> GetFullByIdAsync(Guid id);
    Task<bool> DoesAnyRecipeContainsProductByIdAsync(Guid id);
}
