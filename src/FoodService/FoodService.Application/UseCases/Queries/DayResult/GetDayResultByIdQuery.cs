using FoodService.Application.DTOs.DayResult.Responses;

namespace FoodService.Application.UseCases.Queries.DayResult
{
    public record GetDayResultByIdQuery(Guid ProfileId, Guid UserId, Guid DayResultId)
        : IQuery<DayResultResponse>;
}
