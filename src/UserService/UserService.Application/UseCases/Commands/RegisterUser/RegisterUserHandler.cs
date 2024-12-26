using UserService.Application.Models;
using AutoMapper;
using UserService.Domain.Entities;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;

namespace UserService.Application.UseCases.Commands
{
    public class RegisterUserHandler : ICommandHandler<RegisterUserCommand, AuthenticatedResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public RegisterUserHandler(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<AuthenticatedResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(request.createUserDto.Email);
            if (existingUser != null)
            {
                throw new AlreadyExists("A user with the same email already exists.");
            }

            var user = _mapper.Map<User>(request.createUserDto);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(10);

            _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();

            var accessToken = _tokenService.GenerateAccessToken(user);

            return new AuthenticatedResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
