using UserService.Application.DTOs.Responses.Profile;

namespace UserService.Application.UseCases.Queries
{
    public record CalculateDailyNutrientsQuery(Guid profileId, Guid userId) : IQuery<DailyNeedsResponse>;
}
