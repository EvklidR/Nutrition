namespace UserService.Application.UseCases.Commands.Profile.IncreaseDesiredGlassesOfWater;

public record ChangeDesiredGlassesOfWaterCommand(int DesiredGlassesOfWater, Guid ProfileId, Guid UserId) : ICommand;
