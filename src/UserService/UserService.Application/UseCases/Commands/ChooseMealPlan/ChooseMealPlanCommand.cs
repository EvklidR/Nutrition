namespace UserService.Application.UseCases.Commands
{
    public record ChooseMealPlanCommand(Guid mealPlanId, Guid profileId, Guid userId) : ICommand;
}
