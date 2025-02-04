using FoodService.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace FoodService.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDatabase;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisDatabase = connectionMultiplexer.GetDatabase();
        }

        public async Task<(bool isFound, TResponse? data)> GetCachedAsync<TResponse>(string cacheKey)
        {
            var cachedData = await _redisDatabase.StringGetAsync(cacheKey);

            if (cachedData.IsNullOrEmpty)
            {
                return (false, default);
            }

            var deserializedCachedData = JsonSerializer.Deserialize<TResponse>(cachedData!);

            return (true, deserializedCachedData);
        }

        public async Task WriteAsync<TRequest>(string cacheKey, TRequest item)
        {
            var serializedData = JsonSerializer.Serialize(item);

            await _redisDatabase.StringSetAsync(cacheKey, serializedData, TimeSpan.FromMinutes(10));
        }
    }
}
