using UserService.Application.Models;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;

namespace UserService.Application.UseCases.Commands
{
    public class LoginUserHandler : ICommandHandler<LoginUserCommand, AuthenticatedResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public LoginUserHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<AuthenticatedResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.email);

            if (user == null)
            {
                throw new Unauthorized("User with such email not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.password, user.HashedPassword))
            {
                throw new Unauthorized("Password and login don't match");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(10);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new AuthenticatedResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
