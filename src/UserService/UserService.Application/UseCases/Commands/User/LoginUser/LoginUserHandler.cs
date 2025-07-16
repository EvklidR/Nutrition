using Microsoft.AspNetCore.Identity;
using UserService.Application.DTOs.Responses.User;
using UserService.Contracts.Exceptions;
using UserService.Contracts.Services;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Commands;

public class LoginUserHandler : ICommandHandler<LoginUserCommand, AuthenticatedResponse>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;

    public LoginUserHandler(ITokenService tokenService, UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<AuthenticatedResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            throw new Unauthorized("Login failed. Invalid email or password");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
        {
            throw new Unauthorized("Login failed. Invalid email or password");
        }

        var accessToken = await _tokenService.GenerateAccessTokenAsync(user);

        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user, cancellationToken);

        return new AuthenticatedResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }
}
