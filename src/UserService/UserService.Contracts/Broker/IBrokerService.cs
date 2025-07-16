using UserService.Contracts.Broker.Enums;

namespace UserService.Contracts.Broker;

public interface IBrokerService
{
    Task PublishMessageAsync(
        string message,
        QueueName queueName,
        string exchange = "",
        bool mandatory = true,
        CancellationToken cancellationToken = default);
}
