namespace UserService.Application.UseCases.Commands
{
    public record ChooseMealPlanCommand(string mealPlanId, Guid profileId, Guid userId) : ICommand;
}
