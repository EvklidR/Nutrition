using UserService.Application.DTOs.Responses.Profile;

namespace UserService.Application.UseCases.Queries;

public record GetUserProfilesQuery(Guid UserId) : IQuery<IEnumerable<ShortProfileResponse>>;
