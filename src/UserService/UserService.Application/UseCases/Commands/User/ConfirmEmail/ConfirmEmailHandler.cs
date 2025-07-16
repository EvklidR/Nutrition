using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using UserService.Contracts.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Commands;

public class ConfirmEmailHandler : ICommandHandler<ConfirmEmailCommand>
{
    private readonly UserManager<User> _userManager;

    public ConfirmEmailHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            throw new Unauthorized("User not found");
        }

        string code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        }
        catch (FormatException)
        {
            throw new BadRequest("Invalid code");
        }

        IdentityResult result;

        if (string.IsNullOrEmpty(request.ChangedEmail))
        {
            result = await _userManager.ConfirmEmailAsync(user, code);
        }
        else
        {
            result = await _userManager.ChangeEmailAsync(user, request.ChangedEmail, code);

            if (result.Succeeded)
            {
                result = await _userManager.SetUserNameAsync(user, request.ChangedEmail);
            }
        }

        if (!result.Succeeded)
        {
            throw new BadRequest("Token  doesn't match");
        }
    }
}
