using Grpc.Core;
using Grpc.Net.Client;
using UserService.Application.Interfaces;
using UserService.Application.Models;
using UserService.Application.Exceptions;
using UserService.Grpc;

namespace UserService.Infrastructure.gRPC
{
    class MealPlanService : IMealPlanService
    {
        private readonly GRPCMealPlanService.GRPCMealPlanServiceClient _client;

        public MealPlanService(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new GRPCMealPlanService.GRPCMealPlanServiceClient(channel);
        }

        public async Task<DailyNeedsResponse> GetDailyNeedsByMealPlanAsync(
            Guid userId,
            double bodyWeight,
            double dailyKcal)
        {
            var request = new GetKcalAndMacrosRequest
            {
                ProfileId = userId.ToString(),
                BodyWeight = bodyWeight,
                DailyKcal = dailyKcal
            };

            try
            {
                var response = await _client.CalculateKcalAndMacrosAsync(request);

                return new DailyNeedsResponse(response.Calories, response.Proteins, response.Fats, response.Carbohydrates);
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
