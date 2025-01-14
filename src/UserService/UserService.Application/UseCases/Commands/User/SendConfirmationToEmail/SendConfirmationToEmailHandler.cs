using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Commands
{
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
            var code = request.isChange
                ? await _userManager.GenerateChangeEmailTokenAsync(request.user, request.email)
                : await _userManager.GenerateEmailConfirmationTokenAsync(request.user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var userId = request.user.Id;

            var confirmEmailUrl = $"{request.url}/User/confirmEmail?userId={userId}&code={code}";

            if (request.isChange)
            {
                confirmEmailUrl += $"&changedEmail={request.email}";
            }

            await _emailService.SendConfirmationEmailAsync(request.email, $"<a href='{confirmEmailUrl}'>Confirm</a>");
        }
    }
}
