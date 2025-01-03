using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendConfirmationEmailAsync(string email, string link)
        {
            string message = "Please, press here to confirm your email " + link;
            await SendEmailAsync(email, "Confirmation", message);
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var portString = _configuration["EmailSettings:Port"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            var fromAddress = _configuration["EmailSettings:FromAddress"];

            if (string.IsNullOrEmpty(fromAddress))
            {
                throw new BadRequest("From address must be provided");
            }

            if (string.IsNullOrEmpty(portString) || !int.TryParse(portString, out int port))
            {
                throw new BadRequest("Valid port number must be provided");
            }

            using var client = new SmtpClient(smtpServer, port);
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                throw new BadRequest($"Error sending email: {ex.Message}");
            }
        }
    }
}
