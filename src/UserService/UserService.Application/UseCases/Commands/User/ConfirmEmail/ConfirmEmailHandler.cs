using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Commands
{
    public class ConfirmEmailHandler : ICommandHandler<ConfirmEmailCommand>
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;

        public ConfirmEmailHandler(ITokenService tokenService, UserManager<User> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.userId.ToString());

            if (user == null)
            {
                throw new Unauthorized("User not found");
            }

            string code;

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.code));
            }
            catch (FormatException)
            {
                throw new BadRequest("Invalid code");
            }

            IdentityResult result;

            if (string.IsNullOrEmpty(request.changedEmail))
            {
                result = await _userManager.ConfirmEmailAsync(user, code);
            }
            else
            {
                result = await _userManager.ChangeEmailAsync(user, request.changedEmail, code);

                if (result.Succeeded)
                {
                    result = await _userManager.SetUserNameAsync(user, request.changedEmail);
                }
            }

            if (!result.Succeeded)
            {
                throw new BadRequest("Token  doesn't match");
            }
        }
    }
}
