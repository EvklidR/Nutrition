using MediatR;

namespace FoodService.Application.UseCases.Commands.DayResult
{
    public record DeleteDayResultCommand(Guid DayResultId, Guid UserId) : ICommand;
}
