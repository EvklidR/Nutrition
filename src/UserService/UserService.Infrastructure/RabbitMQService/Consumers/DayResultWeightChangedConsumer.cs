using Microsoft.Extensions.Options;
using UserService.Contracts.Broker.Enums;
using UserService.Infrastructure.RabbitMQService.Settings;

namespace UserService.Infrastructure.RabbitMQService.Consumers;

public class DayResultWeightChangedConsumer : BaseRabbitMQConsumer
{
    public DayResultWeightChangedConsumer(IOptions<RabbitMqSettings> options) : base(options)
    {
        _queueName = QueueName.DayResultWeightChanged.ToString();
    }
}
