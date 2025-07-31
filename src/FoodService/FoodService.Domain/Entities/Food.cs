namespace FoodService.Domain.Entities;

public abstract class Food
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public double Calories { get; set; }
    public double Proteins { get; set; }
    public double Fats { get; set; }
    public double Carbohydrates { get; set; }
}
