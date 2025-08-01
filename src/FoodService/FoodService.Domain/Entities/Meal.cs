using FoodService.Domain.Entities.Interfaces;

namespace FoodService.Domain.Entities;

public class Meal : IHasId
{
    public Guid Id { get; set; }
    public Guid DayId { get; set; }
    public string? Name { get; set; }

    public List<EatenProduct> Products { get; set; } = new List<EatenProduct>();
    public List<EatenDish> Dishes { get; set; } = new List<EatenDish>();
}
