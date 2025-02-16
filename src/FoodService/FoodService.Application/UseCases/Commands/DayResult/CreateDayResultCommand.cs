using FoodService.Application.DTOs.DayResult;

namespace FoodService.Application.UseCases.Commands.DayResult
{
    public record CreateDayResultCommand(CreateDayResultDTO CreateDayResultDTO, Guid UserId) 
        : ICommand<DayResultDTO>;
}
