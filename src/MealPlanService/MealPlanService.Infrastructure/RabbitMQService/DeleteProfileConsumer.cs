using MealPlanService.Infrastructure.Enums;
using MealPlanService.Infrastructure.RabbitMQService.Settings;
using Microsoft.Extensions.Options;

namespace MealPlanService.Infrastructure.RabbitMQService;

public class DeleteProfileConsumer : BaseRabbitMQConsumer
{
    public DeleteProfileConsumer(IOptions<RabbitMqSettings> options) : base(options)
    {
        _queueName = QueueName.ProfileDeletedForMealPlanService;
    }
}
