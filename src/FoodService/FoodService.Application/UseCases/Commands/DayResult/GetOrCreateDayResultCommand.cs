using FoodService.Application.DTOs.DayResult;
using MediatR;

namespace FoodService.Application.UseCases.Commands.DayResult
{
    public record GetOrCreateDayResultCommand(Guid ProfileId, Guid UserId)
        : ICommand<DayResultDTO>;
}
