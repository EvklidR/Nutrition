using UserService.Application.Models;

namespace UserService.Application.UseCases.Queries
{
    public record CalculateDailyNeedsQuery(Guid profileId, Guid userId) : IQuery<DailyNeedsResponse>;
}
