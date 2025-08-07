using Microsoft.Extensions.Options;
using UserService.Infrastructure.RabbitMQService.Settings;
using UserService.Contracts.Broker.Enums;

namespace UserService.Infrastructure.RabbitMQService.Consumers;

public class RabbitMQRevokeMealPlanConsumer : BaseRabbitMQConsumer
{
    public RabbitMQRevokeMealPlanConsumer(IOptions<RabbitMqSettings> options) : base(options)
    {
        _queueName = QueueName.MealPlanRevoked.ToString();
    }
}
