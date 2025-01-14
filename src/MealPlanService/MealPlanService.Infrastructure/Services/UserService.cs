using Grpc.Net.Client;
using MealPlanService.Grpc;

namespace MealPlanService.Infrastructure.Services
{
    public class UserService
    {
        private readonly CheckUserService.CheckUserServiceClient _client;

        public UserService(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new CheckUserService.CheckUserServiceClient(channel);
        }

        public async Task<bool> CheckProfileBelonging(
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
