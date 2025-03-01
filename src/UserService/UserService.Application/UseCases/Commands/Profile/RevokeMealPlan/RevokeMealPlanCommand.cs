namespace UserService.Application.UseCases.Commands
{
    public record RevokeMealPlanCommand(Guid profileId) : ICommand;
}
