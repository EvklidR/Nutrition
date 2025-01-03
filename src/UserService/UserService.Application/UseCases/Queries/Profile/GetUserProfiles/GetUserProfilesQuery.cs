using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Queries
{
    public record GetUserProfilesQuery(Guid userId) : IQuery<IEnumerable<Profile>?>;
}
