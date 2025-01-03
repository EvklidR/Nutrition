namespace FoodService.Domain.Entities
{
    public class EatenFood
    {
        public Guid FoodId { get; set; }
        public Guid MealId { get; set; }
        public double Weight { get; set; }

        public Food Food { get; set; }
    }
}
