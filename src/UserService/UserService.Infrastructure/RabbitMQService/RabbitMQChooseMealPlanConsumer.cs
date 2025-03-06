using Microsoft.Extensions.Options;
using UserService.Infrastructure.RabbitMQService.Settings;
using UserService.Application.Enums;

namespace UserService.Infrastructure.RabbitMQService
{
    public class RabbitMQChooseMealPlanConsumer : BaseRabbitMQConsumer
    {
        public RabbitMQChooseMealPlanConsumer(IOptions<RabbitMqSettings> options) : base(options)
        {
            _queueName = QueueName.MealPlanChoosen.ToString();
        }
    }
}
