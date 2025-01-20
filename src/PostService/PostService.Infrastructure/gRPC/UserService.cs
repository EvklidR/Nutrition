using Grpc.Net.Client;
using PostService.Grpc;
using PostService.Infrastructure.gRPC.Interfaces;

namespace PostService.Infrastructure.gRPC
{
    public  class UserService : IUserService
    {
        private readonly GRPCUserService.GRPCUserServiceClient _client;

        public UserService(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new GRPCUserService.GRPCUserServiceClient(channel);
        }

        public async Task<bool> CheckUserExistence(string userId)
        {
            var request = new CheckUserRequest { UserId = userId };

            try
            {
                var response = await _client.CheckUserAsync(request);

                return response.Exists;
            }
            catch
            {
                return false;
            }
        }

    }
}
