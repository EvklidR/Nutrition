using FoodService.Domain.Entities;
using FoodService.Infrastructure.MSSQL;
using FoodService.Infrastructure.RabbitMQService.Consumers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;

namespace FoodService.Infrastructure.RabbitMQService.Listeners;

public class ChangeProfileWeightListener : BackgroundService
{
    private readonly IServiceScopeFactory _factory;
    private readonly ChangeProfileWeightConsumer _consumer;

    public ChangeProfileWeightListener(IServiceScopeFactory factory, ChangeProfileWeightConsumer consumer)
    {
        _factory = factory;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.AddListenerAsync(async args =>
        {
            using var scope = _factory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var data = JsonConvert.DeserializeObject<WeightChangedMessage>(message);

                if (data is not null)
                {
                    var dayResult = await dbContext.DayResults
                    .Where(dr => 
                        dr.ProfileId == data.ProfileId && 
                        dr.Date == DateOnly.FromDateTime(DateTime.Now))
                    .FirstOrDefaultAsync();

                    if (dayResult != null)
                    {

                        dayResult.Weight = data.NewWeight;

                        dbContext.DayResults.Update(dayResult);
                    }
                    else
                    {
                        dayResult = new DayResult
                        {
                            Weight = data.NewWeight,
                            GlassesOfWater = 0,
                            Date = DateOnly.FromDateTime(DateTime.Now),
                            ProfileId = data.ProfileId
                        };

                        dbContext.DayResults.Add(dayResult);
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} [ERROR] Failed to process message: {ex.Message}");
            }
        });
    }

    private class WeightChangedMessage
    {
        public Guid ProfileId { get; set; }
        public double NewWeight { get; set; }
    }
}


