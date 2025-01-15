using Grpc.Net.Client;
using MealPlanService.Grpc;

namespace MealPlanService.Infrastructure.Services
{
    public class UserService
    {
        private readonly GRPCUserService.GRPCUserServiceClient _client;

        public UserService(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new GRPCUserService.GRPCUserServiceClient(channel);
        }

        public virtual async Task<bool> CheckProfileBelonging(
            string userId,
            string profileId)
        {
            var request = new CheckProfileBelongingRequest
            {
                UserId = userId,
                ProfileId = profileId
            };

            try
            {
                var response = await _client.CheckProfileBelongingAsync(request);

                return response.Belong;
            }
            catch
            {
                return false;
            }
        }

    }
}
