namespace FoodService.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>?> GetAllAsync(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
