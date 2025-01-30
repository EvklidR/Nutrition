using Grpc.Net.Client;
using FoodService.Grpc;
using FoodService.Application.Interfaces;

namespace FoodService.Infrastructure.Grpc
{
    public class UserService : IUserService
    {
        private readonly GRPCUserService.GRPCUserServiceClient _client;

        public UserService(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new GRPCUserService.GRPCUserServiceClient(channel);
        }

        public async Task<bool> CheckProfileBelonging(Guid userId, Guid profileId)
        {
            var request = new CheckProfileBelongingRequest { UserId = userId.ToString(), ProfileId = profileId.ToString() };

            var response = await _client.CheckProfileBelongingAsync(request);

            return response.Belong;
        }

        public async Task<bool> CheckUserByIdAsync(Guid userId)
        {
            var request = new CheckUserRequest { UserId = userId.ToString() };

            var response = await _client.CheckUserAsync(request);

            return response.Exists;
        }
    }
}