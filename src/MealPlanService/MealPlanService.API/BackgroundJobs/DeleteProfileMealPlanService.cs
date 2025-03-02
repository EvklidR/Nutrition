using System.Text;
using System.Text.Json;
using MealPlanService.Infrastructure.Enums;
using MealPlanService.BusinessLogic.Services;
using MealPlanService.Infrastructure.RabbitMQService;

namespace MealPlanService.API.BackgroundJobs
{
    public class DeleteProfileMealPlanService : BackgroundService
    {
        private readonly IServiceScopeFactory _factory;
        private readonly RabbitMQConsumer _consumer;

        public DeleteProfileMealPlanService(IServiceScopeFactory factory, RabbitMQConsumer consumer)
        {
            _factory = factory;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.AddListenerAsync(QueueName.ProfileDeleted.ToString(), async args =>
            {
                using var scope = _factory.CreateScope();

                var profilePlanService = scope.ServiceProvider.GetRequiredService<ProfilePlanService>();

                try
                {
                    var body = args.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    await profilePlanService.DeleteProfilePlansAsync(message);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} [ERROR] Failed to process message: {ex.Message}");
                }
            });
        }
    }
}
