using FoodService.Application.Enums;
using FoodService.Infrastructure.RabbitMQService.Settings;
using Microsoft.Extensions.Options;

namespace FoodService.Infrastructure.RabbitMQService.Consumers;

public class ProfileDeletedConsumer : BaseRabbitMQConsumer
{
    public ProfileDeletedConsumer(IOptions<RabbitMqSettings> options) : base(options)
    {
        _queueName = QueueName.ProfileDeletedForDayResultService;
    }
}
