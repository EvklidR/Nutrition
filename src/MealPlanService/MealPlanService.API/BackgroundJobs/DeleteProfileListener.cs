using System.Text;
using MealPlanService.BusinessLogic.Services;
using MealPlanService.Infrastructure.Enums;
using MealPlanService.Infrastructure.RabbitMQService;

namespace MealPlanService.API.BackgroundJobs
{
    public class DeleteProfileListener : BackgroundService
    {
        private readonly IServiceScopeFactory _factory;
        private readonly DeleteProfileConsumer _consumer;

        public DeleteProfileListener(IServiceScopeFactory factory, DeleteProfileConsumer consumer)
        {
            _factory = factory;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.AddListenerAsync(async args =>
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
            },
            ExchangeName.ProfileDeleted);
        }
    }
}
