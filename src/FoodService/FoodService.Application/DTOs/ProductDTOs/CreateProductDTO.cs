namespace FoodService.Application.DTOs
{
    public class CreateProductDTO
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }
    }
}
