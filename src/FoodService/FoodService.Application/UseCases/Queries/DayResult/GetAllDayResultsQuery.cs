using FoodService.Application.DTOs.DayResult.Responses;
using FoodService.Domain.Interfaces.Repositories.Models;

namespace FoodService.Application.UseCases.Queries.DayResult;

public record GetAllDayResultsQuery(
    Guid ProfileId, 
    Guid UserId,
    PaginatedParameters PaginatedParameters,
    PeriodParameters? PeriodParameters
    ) : IQuery<IEnumerable<ShortDayResultResponse>>;
