namespace FoodService.Application.DTOs.Meal.Requests
{
    public class UpdateMealDTO
    {
        public Guid Id { get; set; }
        public Guid DayResultId { get; set; }
        public string? Name { get; set; }

        public List<CreateOrUpdateEatenProductDTO> Products { get; set; } = [];
        public List<CreateOrUpdateEatenDishDTO> Dishes { get; set; } = [];
    }
}
