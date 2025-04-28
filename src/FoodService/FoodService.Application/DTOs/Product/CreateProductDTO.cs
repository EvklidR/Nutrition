namespace FoodService.Application.DTOs.Product
{
    public class CreateProductDTO
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }
    }
}
