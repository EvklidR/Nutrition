using Microsoft.Extensions.Options;
using UserService.Infrastructure.RabbitMQService.Settings;
using UserService.Application.Enums;

namespace UserService.Infrastructure.RabbitMQService
{
    public class RabbitMQRevokeMealPlanConsumer : BaseRabbitMQConsumer
    {
        public RabbitMQRevokeMealPlanConsumer(IOptions<RabbitMqSettings> options) : base(options)
        {
            _queueName = QueueName.MealPlanRevoked.ToString();
        }
    }
}
