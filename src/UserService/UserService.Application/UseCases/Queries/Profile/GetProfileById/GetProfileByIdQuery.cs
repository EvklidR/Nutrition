using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Queries
{
    public record GetProfileByIdQuery(Guid profileId, Guid userId) : IQuery<Profile>;
}
