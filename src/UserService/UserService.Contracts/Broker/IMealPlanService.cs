namespace UserService.Application.Interfaces;

public interface IMealPlanService
{
    Task<(double Calories, double Proteins, double Fats, double Carbohydrates)> GetDailyNeedsByMealPlanAsync(
        Guid userId,
        double bodyWeight,
        double dailyKcal);
}
