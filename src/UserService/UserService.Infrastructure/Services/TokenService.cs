using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserService.Application.Interfaces;
using System.Text;
using Microsoft.Extensions.Configuration;
using UserService.Application.Exceptions;
using UserService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Hangfire;
using System.Security.Cryptography;
using UserService.Contracts.DataAccess.Repositories;

namespace UserService.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly IRefreshTokenTokenRepository _refreshTokenTokenRepository;

    public TokenService(
        IConfiguration configuration,
        UserManager<User> userManager,
        IRefreshTokenTokenRepository refreshTokenTokenRepository)
    {
        _userManager = userManager;
        _configuration = configuration;
        _refreshTokenTokenRepository = refreshTokenTokenRepository;
    }

    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, roles[0])
        };

        var rawKey = _configuration["AuthOptions:Key"];
        var base64Key = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawKey));
        var secretKey = new SymmetricSecurityKey(Convert.FromBase64String(base64Key));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            _configuration["AuthOptions:Issuer"],
            _configuration["AuthOptions:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: signinCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return tokenString;
    }

    public async Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken)
    {
        string refreshToken = GenerateRandomToken();

        var token = new RefreshToken() { Token = refreshToken, UserId = user.Id };

        await _refreshTokenTokenRepository.AddAsync(token, cancellationToken);

        BackgroundJob.Schedule(
            () => DeleteTokenAsync(token, cancellationToken),
            TimeSpan.FromDays(30));

        return refreshToken;
    }

    private string GenerateRandomToken()
    {
        byte[] randomBytes = new byte[32];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        string refreshToken = Convert.ToBase64String(randomBytes);


        return refreshToken;
    }

    public async Task DeleteTokenAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        await _refreshTokenTokenRepository.DeleteAsync(token, cancellationToken);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthOptions:Key"])),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;

        ClaimsPrincipal principal;

        try
        {
            principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception();
            }
        }
        catch (Exception)
        {
            throw new BadRequest("Access token isn't valid");
        }
        return principal;
    }
}
