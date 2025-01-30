namespace FoodService.Application.DTOs.Dish
{
    public class FullDishDishDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public double Calories { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }
        public double Weight { get; set; }

        public List<DishProductDTO> Ingredients { get; set; }
    }
}
