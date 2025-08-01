using FoodService.Domain.Entities.Interfaces;

namespace FoodService.Domain.Entities;

public class DayResult : IHasId, IHasDate
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public DateOnly Date { get; set; }
    public double Weight { get; set; }
    public int GlassesOfWater { get; set; } = 0;


    public List<Meal> Meals { get; set; } = new List<Meal>();
}
