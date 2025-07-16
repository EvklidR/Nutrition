using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using UserService.Contracts.Exceptions;
using UserService.Contracts.Services;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Commands;

public class SendConfirmationToEmailHandler : ICommandHandler<SendConfirmationToEmailCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    public SendConfirmationToEmailHandler(UserManager<User> userManager, IEmailService emailService)
    {
        _emailService = emailService;
        _userManager = userManager;
    }
    public async Task Handle(SendConfirmationToEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            throw new NotFound("User with such id not found");
        }

        var code = request.IsChange
            ? await _userManager.GenerateChangeEmailTokenAsync(user, request.Email)
            : await _userManager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var confirmEmailUrl = $"{request.Url}/User/confirmEmail?userId={request.UserId}&code={code}";

        if (request.IsChange)
        {
            confirmEmailUrl += $"&changedEmail={request.Email}";
        }

        await _emailService.SendConfirmationEmailAsync(request.Email, $"<a href='{confirmEmailUrl}'>Confirm</a>");
    }
}
