using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Commands
{
    public record SendConfirmationToEmailCommand(User user, string? url, string email, bool isChange = false) : ICommand;
}