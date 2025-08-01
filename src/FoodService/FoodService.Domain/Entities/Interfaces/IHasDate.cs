namespace FoodService.Domain.Entities.Interfaces;

public interface IHasDate
{
    DateOnly Date { get; set; }
}
