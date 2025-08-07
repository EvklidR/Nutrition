using UserService.Contracts.Broker.Enums;

namespace UserService.Contracts.Broker;

public interface IBrokerService
{
    Task PublishMessageAsync(
        string message,
        QueueName? queueName,
        ExchangeName? exchange,
        bool mandatory = true,
        CancellationToken cancellationToken = default);
}
