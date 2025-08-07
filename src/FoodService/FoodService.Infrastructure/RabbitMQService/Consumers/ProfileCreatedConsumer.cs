using FoodService.Application.Enums;
using FoodService.Infrastructure.RabbitMQService.Settings;
using Microsoft.Extensions.Options;

namespace FoodService.Infrastructure.RabbitMQService.Consumers;

public class ProfileCreatedConsumer : BaseRabbitMQConsumer
{
    public ProfileCreatedConsumer(IOptions<RabbitMqSettings> options) : base(options)
    {
        _queueName = QueueName.ProfileCreated;
    }
}
