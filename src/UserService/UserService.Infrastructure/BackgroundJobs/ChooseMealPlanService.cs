using System.Text;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserService.Application.Enums;
using UserService.Infrastructure.RabbitMQService;
using UserService.Application.UseCases.Commands;

namespace UserService.Infrastructure.BackgroundJobs
{
    public class ChooseMealPlanService : BackgroundService
    {
        private readonly IServiceScopeFactory _factory;
        private readonly RabbitMQChooseMealPlanConsumer _consumer;

        public ChooseMealPlanService(IServiceScopeFactory factory, RabbitMQChooseMealPlanConsumer consumer)
        {
            _factory = factory;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.AddListenerAsync(QueueName.MealPlanChoosen.ToString(), async args =>
            {
                using var scope = _factory.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                try
                {
                    var body = args.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    if (Guid.TryParse(message, out var profileId))
                    {
                        var command = new ChooseMealPlanCommand(profileId);

                        await mediator.Send(command, stoppingToken);
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now} [ERROR] Invalid format of message: {message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} [ERROR] Failed to process message: {ex.Message}");
                }
            });
        }
    }
}
