namespace UserService.Contracts.Broker;

public interface IMealPlanService
{
    Task<(double Calories, double Proteins, double Fats, double Carbohydrates)> GetDailyNeedsByMealPlanAsync(
        Guid userId,
        double bodyWeight,
        double dailyKcal);
}
