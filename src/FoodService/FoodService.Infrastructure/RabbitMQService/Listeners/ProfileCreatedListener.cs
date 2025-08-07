using FoodService.Domain.Entities;
using FoodService.Infrastructure.MSSQL;
using FoodService.Infrastructure.RabbitMQService.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;

namespace FoodService.Infrastructure.RabbitMQService.Listeners;

public class ProfileCreatedListener : BackgroundService
{
    private readonly IServiceScopeFactory _factory;
    private readonly ProfileCreatedConsumer _consumer;

    public ProfileCreatedListener(IServiceScopeFactory factory, ProfileCreatedConsumer consumer)
    {
        _factory = factory;
        _consumer = consumer;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.AddListenerAsync(async args =>
        {
            using var scope = _factory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var data = JsonConvert.DeserializeObject<ProfileCreatedMessage>(message);

                if (data is not null)
                {

                    var dayResult = new DayResult
                    {
                        Weight = data.Weight,
                        GlassesOfWater = 0,
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        ProfileId = data.ProfileId
                    };

                    dbContext.DayResults.Add(dayResult);

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} [ERROR] Failed to process message: {ex.Message}");
            }
        });
    }

    private class ProfileCreatedMessage
    {
        public Guid ProfileId { get; set; }
        public double Weight { get; set; }
    }
}
