namespace EventsService.Application.Interfaces
{
    public interface IEmailService
    {
        int GenerateCode();
        void SendEmail(string email, string subject, string body);
    }
}
