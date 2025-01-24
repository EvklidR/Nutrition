using FoodService.Domain.Interfaces.Repositories;

namespace FoodService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IDishRepository DishRepository { get; }
        IIngredientRepository IngredientRepository { get; }
        IDayResultRepository DayResultRepository { get; }
        Task SaveChangesAsync();
    }
}
