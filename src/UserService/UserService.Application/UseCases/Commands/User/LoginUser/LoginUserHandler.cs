using Microsoft.AspNetCore.Identity;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.Models;
using UserService.Application.UseCases.Commands;
using UserService.Domain.Entities;

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
        var user = await _userManager.FindByEmailAsync(request.email);

        if (user == null)
        {
            throw new Unauthorized("Login failed. Invalid email or password");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.password);
        if (!passwordValid)
        {
            throw new Unauthorized("Login failed. Invalid email or password");
        }

        var accessToken = await _tokenService.GenerateAccessToken(user);
        var refreshToken = await _tokenService.GenerateRefreshToken(user);

        return new AuthenticatedResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }
}
