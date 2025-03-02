using MealPlanService.Infrastructure.Enums;

namespace MealPlanService.Infrastructure.RabbitMQService
{
    public interface IBrokerService
    {
        Task PublishMessageAsync(
            string message,
            QueueName queueName,
            string exchange = "",
            bool mandatory = true,
            CancellationToken cancellationToken = default);
    }
}
