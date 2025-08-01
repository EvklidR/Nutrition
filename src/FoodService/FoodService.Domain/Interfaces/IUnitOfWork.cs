using FoodService.Domain.Interfaces.Repositories;

namespace FoodService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IDishRepository DishRepository { get; }
        IProductRepository ProductRepository { get; }
        IDayResultRepository DayResultRepository { get; }
        IRecipeRepository RecipeRepository { get; }
        Task SaveChangesAsync();
    }
}
