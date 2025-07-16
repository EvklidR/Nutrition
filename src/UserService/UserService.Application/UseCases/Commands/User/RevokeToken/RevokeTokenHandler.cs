using UserService.Contracts.DataAccess.Repositories;

namespace UserService.Application.UseCases.Commands;

public class RevokeTokenHandler : ICommandHandler<RevokeTokenCommand>
{
    private readonly IRefreshTokenTokenRepository _tokenRepository;

    public RevokeTokenHandler(IRefreshTokenTokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var tokens = await _tokenRepository.GetAllByUserAsync((Guid)request.UserId!, cancellationToken);

        foreach (var token in tokens)
        {
            if (token.Token == request.RefreshToken)
            {
                await _tokenRepository.DeleteAsync(token, cancellationToken);

                break;
            }
        }
    }
}
