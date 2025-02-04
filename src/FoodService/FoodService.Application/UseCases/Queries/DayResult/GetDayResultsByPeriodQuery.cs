using FoodService.Application.DTOs.DayResult;
using MediatR;

namespace FoodService.Application.UseCases.Queries.DayResult
{
    public record GetDayResultsByPeriodQuery(Guid ProfileId, DateOnly StartDate, DateOnly EndDate, Guid UserId)
        : IQuery<IEnumerable<DayResultDTO>?>;
}
