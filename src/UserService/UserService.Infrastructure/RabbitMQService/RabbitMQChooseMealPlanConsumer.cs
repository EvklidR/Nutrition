using Microsoft.Extensions.Options;
using UserService.Contracts.Broker.Enums;
using UserService.Infrastructure.RabbitMQService.Settings;

namespace UserService.Infrastructure.RabbitMQService;

public class RabbitMQChooseMealPlanConsumer : BaseRabbitMQConsumer
{
    public RabbitMQChooseMealPlanConsumer(IOptions<RabbitMqSettings> options) : base(options)
    {
        _queueName = QueueName.MealPlanChoosen.ToString();
    }
}
