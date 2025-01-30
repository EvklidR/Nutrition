using MediatR;
using FoodService.Application.DTOs.DayResult;

namespace FoodService.Application.UseCases.Commands.DayResult
{
    public record UpdateDayResultCommand(UpdateDayResultDTO UpdateDayResultDTO, Guid UserId) : ICommand;
}
