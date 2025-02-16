namespace FoodService.Domain.Entities
{
    public class EatenFood
    {
        public Guid Id { get; set; }
        public Guid FoodId { get; set; }
        public Guid MealId { get; set; }
        public double Weight { get; set; }

        public Food Food { get; set; }
    }

    public class EatenProduct : EatenFood
    {
    }

    public class EatenDish : EatenFood
    {
    }
}
