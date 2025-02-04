namespace FoodService.Application.UseCases.Queries.Dish
{
    public record GetDishImageQuery(Guid DishId) : IQuery<Stream>;
}
