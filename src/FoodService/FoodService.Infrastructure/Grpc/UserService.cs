using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Grpc;
using Grpc.Core;
using Grpc.Net.Client;

namespace FoodService.Infrastructure.Grpc
{
    public class UserService : IUserService
    {
        private readonly GRPCUserService.GRPCUserServiceClient _client;
        private readonly ICacheService _cacheService;

        public UserService(string address, ICacheService cacheService)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new GRPCUserService.GRPCUserServiceClient(channel);
            _cacheService = cacheService;
        }

        public async Task CheckProfileBelongingAsync(Guid userId, Guid profileId)
        {
            var cacheKey = userId.ToString() + "_" + profileId.ToString();

            var cachedResponse = await _cacheService.GetCachedAsync<bool>(cacheKey);

            if (cachedResponse.isFound)
            {
                if (!cachedResponse.data)
                {
                    throw new Forbidden("Profile doesn't belong to this user");
                }

                return;
            }

            var request = new CheckProfileBelongingRequest { UserId = userId.ToString(), ProfileId = profileId.ToString() };

            var response = await _client.CheckProfileBelongingAsync(request);

            await _cacheService.WriteAsync(cacheKey, response.Belong);

            if (!response.Belong)
            {
                throw new Forbidden("Profile doesn't belong to this user");
            }
        }

        public async Task CheckUserByIdAsync(Guid userId)
        {
            var cachedResponse = await _cacheService.GetCachedAsync<bool>(userId.ToString());

            if (cachedResponse.isFound)
            {
                if (!cachedResponse.data)
                {
                    throw new NotFound("User not found");
                }

                return;
            }

            var request = new CheckUserRequest { UserId = userId.ToString() };

            var response = await _client.CheckUserAsync(request);

            await _cacheService.WriteAsync(userId.ToString(), response.Exists);

            if (!response.Exists)
            {
                throw new NotFound("User not found");
            }
        }

        public async Task<double> GetProfileWeightAsync(Guid profileId)
        {
            var cachedResponse = await _cacheService.GetCachedAsync<double>("weight-" + profileId.ToString());

            if (cachedResponse.isFound)
            {
                return cachedResponse.data;
            }

            var request = new GetProfileWeightRequest { ProfileId = profileId.ToString() };

            try
            {
                var response = await _client.GetProfileWeightAsync(request);

                await _cacheService.WriteAsync("weight-" + profileId.ToString(), response.Weight);

                return response.Weight;
            }
            catch (RpcException ex)
            {
                switch (ex.StatusCode)
                {
                    case StatusCode.NotFound:
                        throw new NotFound(ex.Message);
                    case StatusCode.InvalidArgument:
                        throw new BadRequest(ex.Message);
                    default:
                        throw new Exception("An error occurred while calling the gRPC service.", ex);
                }
            }
        }
    }
}