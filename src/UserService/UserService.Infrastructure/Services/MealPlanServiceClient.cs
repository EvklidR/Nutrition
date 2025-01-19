using Grpc.Net.Client;
using UserService.Grpc;
using UserService.Application.Interfaces;
using UserService.Application.Models;

namespace UserService.Infrastructure.Grpc
{
    public class MealPlanServiceClient : IMealPlanService
    {
        private readonly MealPlanService.MealPlanServiceClient _client;

        public MealPlanServiceClient(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new MealPlanService.MealPlanServiceClient(channel);
        }

        public async Task<DailyNeedsResponse> GetDailyNeedsByMealPlanAsync(
            Guid profileId,
            double bodyWeight,
            double dailyKcal)
        {
            var request = new GetKcalAndMacrosRequest
            {
                ProfileId = profileId.ToString(),
                BodyWeight = bodyWeight,
                DailyKcal = dailyKcal
            };

            var response = await _client.CalculateKcalAndMacrosAsync(request);

            return new DailyNeedsResponse(response.Calories, response.Proteins, response.Fats, response.Carbohydrates);
        }
    }
}