using FoodService.Application.DTOs.Dish;

namespace FoodService.Application.Models
{
    public class DishesResponse
    {
        public List<BriefDishDTO>? Dishes { get; set; }
        public long TotalCount { get; set; }
    }
}
