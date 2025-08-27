namespace MealPlanService.BusinessLogic.DTOs;

public class ProfileMealPlansResponse
{
    public List<ProfileMealPlanWithDetailsDto> ProfileMealPlans { get; set; } = [];
    public long TotalCount { get; set; }
}
