using MealPlanService.Infrastructure.Enums;

namespace MealPlanService.Infrastructure.RabbitMQService
{
    public interface IBrokerService
    {
        Task PublishMessageAsync(
            string message,
            QueueName? queueName,
            ExchangeName? exchangeName,
            bool mandatory = true,
            CancellationToken cancellationToken = default);
    }
}
