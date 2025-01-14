namespace UserService.Application.UseCases.Queries
{
    public record CheckProfileBelongingQuery(Guid userId, Guid profileId) : IQuery<bool>;
}
