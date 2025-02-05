using FoodService.Application.DTOs.DayResult;

namespace FoodService.Application.UseCases.Commands.DayResult
{
    public record GetOrCreateDayResultCommand(Guid ProfileId, Guid UserId)
        : ICommand<DayResultDTO>;
}
