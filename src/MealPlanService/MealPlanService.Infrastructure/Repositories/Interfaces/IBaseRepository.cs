namespace MealPlanService.Infrastructure.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T updatedEntity);
        Task DeleteAsync(string id);
    }
}
