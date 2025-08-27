namespace FoodService.Application.DTOs.DayResult.Responses;

public class DayResultsResponse
{
    public List<ShortDayResultResponse> DayResults { get; set; } = [];
    public long TotalCount { get; set; }
}
