using FoodService.Application.DTOs.DayResult;
using FoodService.Application.DTOs.DayResult.Requests;

namespace FoodService.Application.UseCases.Commands.DayResult
{
    public record CreateDayResultCommand(CreateDayResultDTO CreateDayResultDTO, Guid UserId) 
        : ICommand<DayResultDTO>;
}
