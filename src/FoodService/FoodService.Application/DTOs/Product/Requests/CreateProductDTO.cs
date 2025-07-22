namespace FoodService.Application.DTOs.Product.Requests
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = null!;
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }
    }
}
