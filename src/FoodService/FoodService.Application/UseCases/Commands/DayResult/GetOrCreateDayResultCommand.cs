using FoodService.Application.DTOs.DayResult.Responses;

namespace FoodService.Application.UseCases.Commands.DayResult
{
    public record GetOrCreateDayResultCommand(Guid ProfileId, Guid UserId)
        : ICommand<DayResultResponse>;
}
