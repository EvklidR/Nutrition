namespace FoodService.Domain.Entities
{
    public class Meal
    {
        public Guid Id { get; set; }
        public Guid DayId { get; set; }
        public string Name { get; set; }

        public List<EatenProduct> Products { get; set; } = [];
        public List<EatenDish> Dishes { get; set; } = [];
    }
}
