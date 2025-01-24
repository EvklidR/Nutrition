namespace MealPlanService.BusinessLogic.Models
{
    public class RequestForCalculating
    {
        public string ProfileId { get; set; }
        public double BodyWeight { get; set; }
        public double DailyKcal { get; set; }
    }
}
