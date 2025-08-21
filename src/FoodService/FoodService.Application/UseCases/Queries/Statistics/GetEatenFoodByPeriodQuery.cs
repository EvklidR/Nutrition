using FoodService.Application.DTOs.Statistics;
using FoodService.Domain.Interfaces.Repositories.Models;

namespace FoodService.Application.UseCases.Queries.Statistics;

public record GetEatenFoodByPeriodQuery(Guid ProfileId, Guid UserId, PeriodParameters PeriodParameters) : IQuery<IEnumerable<EatenFoodResponse>>;
