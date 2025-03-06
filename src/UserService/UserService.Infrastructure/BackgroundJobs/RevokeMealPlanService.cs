using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserService.Infrastructure.RabbitMQService;
using System.Text;
using UserService.Application.UseCases.Commands;

namespace UserService.Infrastructure.BackgroundJobs
{
    public class RevokeMealPlanService : BackgroundService
    {
        private readonly IServiceScopeFactory _factory;
        private readonly RabbitMQRevokeMealPlanConsumer _consumer;

        public RevokeMealPlanService(IServiceScopeFactory factory, RabbitMQRevokeMealPlanConsumer consumer)
        {
            _factory = factory;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.AddListenerAsync(async args =>
            {
                using var scope = _factory.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                try
                {
                    var body = args.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    if (Guid.TryParse(message, out var profileId))
                    {
                        var command = new RevokeMealPlanCommand(profileId);

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
