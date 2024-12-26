using UserService.Application.Exceptions;
using UserService.Application.Models;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;

namespace UserService.Application.UseCases.Commands
{
    public class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, AuthenticatedResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public RefreshTokenHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<AuthenticatedResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.accessToken);
            var username = principal.Identity?.Name;

            if (username == null)
            {
                throw new Unauthorized("AccessToken isn't valid");
            }

            var user = await _unitOfWork.UserRepository.GetByEmailAsync(username!);
            if (user == null)
            {
                throw new Unauthorized("User not found");
            }

            if (user.RefreshToken != request.refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new Unauthorized("RefreshToken isn't valid");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);

            return new AuthenticatedResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = user.RefreshToken
            };
        }
    }
}
