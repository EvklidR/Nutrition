public class ProfileMealPlanWithDetailsDto
{
    public string Id { get; set; }
    public string ProfileId { get; set; }
    public string MealPlanId { get; set; }
    public bool IsActive { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public string MealPlanName { get; set; }
    public string MealPlanDescription { get; set; }
}
