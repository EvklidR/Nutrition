namespace FoodService.Application.Interfaces
{
    public interface ICacheService
    {
        Task<(bool isFound, TResponse? data)> GetCachedAsync<TResponse>(string cacheKey);
        Task WriteAsync<TRequest>(string cacheKey, TRequest item);
    }
}
