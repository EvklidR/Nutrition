using UserService.Application.Enums;

namespace UserService.Application.Interfaces;

public interface IBrokerService
{
    Task PublishMessageAsync(
        string message,
        QueueName queueName,
        string exchange = "",
        bool mandatory = true,
        CancellationToken cancellationToken = default);
}
