using FoodService.Application.Enums;
using FoodService.Infrastructure.MSSQL;
using FoodService.Infrastructure.RabbitMQService.Consumers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace FoodService.Infrastructure.RabbitMQService.Listeners;

public class ProfileDeletedListener : BackgroundService
{
    private readonly IServiceScopeFactory _factory;
    private readonly ProfileDeletedConsumer _consumer;

    public ProfileDeletedListener(IServiceScopeFactory factory, ProfileDeletedConsumer consumer)
    {
        _factory = factory;
        _consumer = consumer;
    }

    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _consumer.AddListenerAsync(async args =>
        {
            using var scope = _factory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                if (message is not null && Guid.TryParse(message, out Guid profileId))
                {
                    var dayResults = await dbContext.DayResults.Where(d => d.ProfileId == profileId).ToListAsync();

                    dbContext.DayResults.RemoveRange(dayResults);

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} [ERROR] Failed to process message: {ex.Message}");
            }
        },
        ExchangeName.ProfileDeleted);
    }
}
