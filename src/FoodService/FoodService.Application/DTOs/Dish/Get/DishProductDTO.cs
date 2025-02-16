namespace FoodService.Application.DTOs.Dish
{
    public class DishProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }
        public double WeightPerPortion { get; set; }
    }
}
