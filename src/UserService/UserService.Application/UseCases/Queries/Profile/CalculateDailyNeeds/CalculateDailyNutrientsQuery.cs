using UserService.Application.Models;

namespace UserService.Application.UseCases.Queries
{
    public record CalculateDailyNutrientsQuery(Guid profileId, Guid userId) : IQuery<DailyNeedsResponse>;
}
