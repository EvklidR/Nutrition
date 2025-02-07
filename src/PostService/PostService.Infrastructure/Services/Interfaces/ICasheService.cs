namespace PostService.Infrastructure.Services.Interfaces
{
    public interface ICacheService
    {
        Task<Stream?> GetCachedImageAsync(string cacheKey);
        Task WriteImageAsync(string cacheKey, byte[] imageStream);
    }
}
