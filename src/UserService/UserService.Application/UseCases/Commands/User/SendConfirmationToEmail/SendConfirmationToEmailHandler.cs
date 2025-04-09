using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Application.Exceptions;

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
            var user = await _userManager.FindByIdAsync(request.userId.ToString());

            if (user == null)
            {
                throw new NotFound("User with such id not found");
            }

            var code = request.isChange
                ? await _userManager.GenerateChangeEmailTokenAsync(user, request.email)
                : await _userManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var confirmEmailUrl = $"{request.url}/User/confirmEmail?userId={request.userId}&code={code}";

            if (request.isChange)
            {
                confirmEmailUrl += $"&changedEmail={request.email}";
            }

            await _emailService.SendConfirmationEmailAsync(request.email, $"<a href='{confirmEmailUrl}'>Confirm</a>");
        }
    }
}
