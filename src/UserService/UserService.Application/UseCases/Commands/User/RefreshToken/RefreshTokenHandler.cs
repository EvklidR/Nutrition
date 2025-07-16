using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;
using UserService.Application.DTOs.Responses.User;

namespace UserService.Application.UseCases.Commands
{
    public class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, AuthenticatedResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenTokenRepository _refreshTokenRepository;

        public RefreshTokenHandler(UserManager<User> userManager, ITokenService tokenService, IRefreshTokenTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthenticatedResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            var username = principal.Identity?.Name;

            if (username == null)
            {
                throw new Unauthorized("AccessToken isn't valid");
            }

            var user = await _userManager.FindByEmailAsync(username!);

            if (user == null)
            {
                throw new Unauthorized("User not found");
            }

            var refreshTokens = await _refreshTokenRepository.GetAllByUserAsync(user.Id);

            if (!refreshTokens.Any(x => x.Token == request.RefreshToken))
            {
                throw new Unauthorized("There is no refresh token");
            }

            var newAccessToken = await _tokenService.GenerateAccessToken(user);

            return new AuthenticatedResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = request.RefreshToken
            };
        }
    }
}
