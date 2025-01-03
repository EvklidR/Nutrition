namespace UserService.Application.UseCases.Queries
{
    public record CheckUserByIdQuery(Guid userId) : IQuery<bool>;
}
