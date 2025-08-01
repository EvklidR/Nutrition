namespace FoodService.Domain.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync(Guid id);
    Task<bool> CheckIfAllEntitiesExistAsync(IEnumerable<Guid> ids);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}
