namespace UserService.Contracts.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string body);
    Task SendConfirmationEmailAsync(string email, string link);
}
