namespace UserService.Application.UseCases.Commands
{
    public record RevokeMealPlanCommand(Guid profileId, Guid userId) : ICommand;
}
