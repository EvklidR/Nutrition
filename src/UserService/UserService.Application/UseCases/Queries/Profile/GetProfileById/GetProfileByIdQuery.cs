using UserService.Application.DTOs.Responces.Profile;

namespace UserService.Application.UseCases.Queries;

public record GetProfileByIdQuery(Guid profileId, Guid userId) : IQuery<ProfileResponseDto>;
