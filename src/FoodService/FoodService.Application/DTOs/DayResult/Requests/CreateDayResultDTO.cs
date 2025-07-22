namespace FoodService.Application.DTOs.DayResult.Requests
{
    public class CreateDayResultDTO
    {
        public Guid ProfileId { get; set; }
        public DateOnly Date { get; set; }
    }
}
