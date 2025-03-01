namespace UserService.Application.UseCases.Commands
{
    public record ChooseMealPlanCommand(Guid profileId) : ICommand;
}
