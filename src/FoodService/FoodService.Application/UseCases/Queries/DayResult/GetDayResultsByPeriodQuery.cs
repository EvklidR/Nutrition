using FoodService.Application.DTOs.DayResult.Responses;

namespace FoodService.Application.UseCases.Queries.DayResult
{
    public record GetDayResultsByPeriodQuery(Guid ProfileId, DateOnly StartDate, DateOnly EndDate, Guid UserId)
        : IQuery<IEnumerable<DayResultResponse>?>;
}
