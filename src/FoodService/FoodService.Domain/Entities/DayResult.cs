namespace FoodService.Domain.Entities
{
    public class DayResult
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public DateOnly Date { get; set; }
        public double Weight { get; set; }
        public int GlassesOfWater { get; set; } = 0;


        public List<Meal> Meals { get; set; } = [];
    }
}
