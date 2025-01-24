namespace FoodService.Application.DTOs
{
    public class CreateMealDTO
    {
        public Guid DayResultId { get; set; }
        public string Name { get; set; }
        public List<CreateOrUpdateEatenFoodDTO> Foods { get; set; } = new List<CreateOrUpdateEatenFoodDTO>();
    }
}
