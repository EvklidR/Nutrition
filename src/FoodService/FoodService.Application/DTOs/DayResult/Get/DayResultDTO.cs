using FoodService.Application.DTOs.Meal;

namespace FoodService.Application.DTOs.DayResult
{
    public class DayResultDTO
    {
        public Guid Id { get; set; }
        public int GlassesOfWater { get; set; }
        public DateOnly Date { get; set; } 

        public List<BriefMealDTO> Meals { get; set; } = [];
    }
}
