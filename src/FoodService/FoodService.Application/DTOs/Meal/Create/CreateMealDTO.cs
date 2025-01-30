namespace FoodService.Application.DTOs.Meal
{
    public class CreateMealDTO
    {
        public Guid DayId { get; set; }
        public string Name { get; set; }

        public List<CreateOrUpdateEatenFoodDTO> Products { get; set; } = new List<CreateOrUpdateEatenFoodDTO>();
        public List<CreateOrUpdateEatenFoodDTO> Dishes { get; set; } = new List<CreateOrUpdateEatenFoodDTO>();
    }
}
