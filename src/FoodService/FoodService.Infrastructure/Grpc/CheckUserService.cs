using Grpc.Net.Client;
using FoodService.Grpc;
using FoodService.Application.Interfaces;

namespace FoodService.Infrastructure.Grpc
{
    public class CheckUserService : ICheckUserService
    {
        private readonly GRPCUserService.GRPCUserServiceClient _client;
        private readonly ICacheService _cacheService;

        public CheckUserService(string address, ICacheService cacheService)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new GRPCUserService.GRPCUserServiceClient(channel);
            _cacheService = cacheService;
        }

        public async Task<bool> CheckProfileBelonging(Guid userId, Guid profileId)
        {
            var cacheKey = userId.ToString() + "_" + profileId.ToString();

            var cachedResponse = await _cacheService.GetCachedAsync<bool>(cacheKey);

            if (cachedResponse.isFound)
            {
                return cachedResponse.data;
            }

            var request = new CheckProfileBelongingRequest { UserId = userId.ToString(), ProfileId = profileId.ToString() };

            var response = await _client.CheckProfileBelongingAsync(request);

            await _cacheService.WriteAsync(cacheKey, response.Belong);

            return response.Belong;
        }

        public async Task<bool> CheckUserByIdAsync(Guid userId)
        {
            var cachedResponse = await _cacheService.GetCachedAsync<bool>(userId.ToString());

            if (cachedResponse.isFound)
            {
                return cachedResponse.data;
            }

            var request = new CheckUserRequest { UserId = userId.ToString() };

            var response = await _client.CheckUserAsync(request);

            await _cacheService.WriteAsync(userId.ToString(), response.Exists);

            return response.Exists;
        }
    }
}