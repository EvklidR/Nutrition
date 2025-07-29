using FoodService.Application.DTOs.DayResult.Requests;
using FoodService.Application.DTOs.DayResult.Responses;

namespace FoodService.Application.UseCases.Commands.DayResult
{
    public record CreateDayResultCommand(CreateDayResultDTO CreateDayResultDTO, Guid UserId) 
        : ICommand<DayResultResponse>;
}
