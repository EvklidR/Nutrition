using UserService.Application.DTOs.Responses.Profile;

namespace UserService.Application.UseCases.Queries;

public record GetProfileByIdQuery(Guid profileId, Guid userId) : IQuery<ProfileResponse>;
