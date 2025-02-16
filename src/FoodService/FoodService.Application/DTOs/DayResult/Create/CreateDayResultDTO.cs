namespace FoodService.Application.DTOs.DayResult
{
    public class CreateDayResultDTO
    {
        public Guid ProfileId { get; set; }
        public DateOnly Date { get; set; }
        public double? Weight { get; set; }
        public int GlassesOfWater { get; set; } = 0;
    }
}
