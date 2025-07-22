using FoodService.Application.DTOs.DayResult.Requests;

namespace FoodService.Application.UseCases.Commands.DayResult
{
    public record UpdateDayResultCommand(UpdateDayResultDTO UpdateDayResultDTO, Guid UserId) : ICommand;
}
