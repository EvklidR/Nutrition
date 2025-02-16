namespace FoodService.Application.DTOs.DayResult
{
    public class UpdateDayResultDTO
    {
        public Guid Id { get; set; }
        public double? Weight { get; set; }
        public int GlassesOfWater { get; set; }
    }
}
