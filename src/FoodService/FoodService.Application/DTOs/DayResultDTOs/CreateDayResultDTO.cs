namespace FoodService.Application.DTOs
{
    public class CreateDayResultDTO
    {
        public string UserId { get; set; }
        public DateOnly Date { get; set; }
        public double? Weight { get; set; }
        public int GlassesOfWater { get; set; } = 0;
    }
}
