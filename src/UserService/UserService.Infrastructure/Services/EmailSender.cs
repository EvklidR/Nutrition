using System.Net.Mail;
using System.Net;
using EventsService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using EventsService.Application.Exceptions;


namespace EventsService.Infrastructure.Services
{
    public class EmailSender : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string email, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var portString = _configuration["EmailSettings:Port"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            var fromAddress = _configuration["EmailSettings:FromAddress"];

            if (string.IsNullOrEmpty(fromAddress))
            {
                throw new BadRequest("From address must be provided.");
            }

            if (string.IsNullOrEmpty(portString) || !int.TryParse(portString, out int port))
            {
                throw new BadRequest("Valid port number must be provided.");
            }

            using (var client = new SmtpClient(smtpServer, port))
            {
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

                client.Send(mailMessage);
            }
        }
    }
}
