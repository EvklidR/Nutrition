using FoodService.Application.Enums;

namespace FoodService.Application.Interfaces;

public interface IBrokerService
{
    Task PublishMessageAsync(
        string message,
        QueueName? queueName,
        ExchangeName? exchange,
        bool mandatory = true,
        CancellationToken cancellationToken = default);
}
