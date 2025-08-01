using FoodService.Application.DTOs.DayResult.Responses;
using FoodService.Domain.Interfaces.Repositories.Models;

namespace FoodService.Application.UseCases.Queries.DayResult
{
    public record GetStatisticsByPeriodQuery(Guid ProfileId, PeriodParameters PeriodParameters, Guid UserId)
        : IQuery<IEnumerable<ShortDayResultResponse>>;
}
