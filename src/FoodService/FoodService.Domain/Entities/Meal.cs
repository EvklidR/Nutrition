namespace FoodService.Domain.Entities
{
    public class Meal
    {
        public Guid Id { get; set; }
        public Guid DayId { get; set; }
        public string Name { get; set; }

        public List<EatenFood> Foods { get; set; } = new List<EatenFood>();
    }
}
