using PostService.Infrastructure.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace PostService.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDatabase;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisDatabase = connectionMultiplexer.GetDatabase();
        }

        public async Task<Stream?> GetCachedImageAsync(string cacheKey)
        {
            var cachedData = await _redisDatabase.StringGetAsync(cacheKey);

            if (cachedData.IsNullOrEmpty)
            {
                return null;
            }

            var bytes = JsonSerializer.Deserialize<byte[]>(cachedData!);

            return new MemoryStream(bytes!);
        }

        public async Task WriteImageAsync(string cacheKey, byte[] imageStream)
        {
            var serializedData = JsonSerializer.Serialize(imageStream);

            await _redisDatabase.StringSetAsync(cacheKey, serializedData, TimeSpan.FromMinutes(10));
        }
    }
}
