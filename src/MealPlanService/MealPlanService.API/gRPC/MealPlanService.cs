using Grpc.Core;
using MealPlanService.BusinessLogic.Exceptions;
using MealPlanService.BusinessLogic.Models;
using MealPlanService.BusinessLogic.Services;
using MealPlanService.Grpc;

namespace MealPlanService.API.gRPC
{
    public class MealPlanService : GrpcMealPlanService.GrpcMealPlanServiceBase
    { 
        private readonly ProfilePlanService _userPlanService;

        public MealPlanService(ProfilePlanService userPlanService)
        {
            _userPlanService = userPlanService;
        }

        public override async Task<CalculateKcalAndMacrosResponse> CalculateKcalAndMacros(CalculateKcalAndMacrosRequest request, ServerCallContext context)
        {
            var requestForCalc = new RequestForCalculating()
            { 
                ProfileId = request.ProfileId,
                BodyWeight = request.BodyWeight,
                DailyKcal = request.DailyKcal
            };
            var responce = new CalculateKcalAndMacrosResponse();
            try
            {
                var resp = await _userPlanService.CalculateKcalAndMacros(requestForCalc);

                responce.Calories = resp.Calories;
                responce.Proteins = resp.Proteins;
                responce.Fats = resp.Fats;
                responce.Carbohydrates = resp.Carbohydrates;
            }
            catch (NotFound ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Unknown, ex.Message));
            }
            return responce;
        }
    }
}
