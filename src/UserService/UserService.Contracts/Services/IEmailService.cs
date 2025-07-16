using System.Globalization;

namespace UserService.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string body);
        Task SendConfirmationEmailAsync(string email, string link);
    }
}
