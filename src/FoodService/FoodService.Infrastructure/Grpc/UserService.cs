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

        public async Task<bool> CheckProfileBelonging(string userId, string profileId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckUserByIdAsync(string userId)
        {
            var request = new CheckUserRequest { UserId = userId.ToString() };

            var response = await _client.CheckUserAsync(request);
            return response.Exists;
        }
    }
}