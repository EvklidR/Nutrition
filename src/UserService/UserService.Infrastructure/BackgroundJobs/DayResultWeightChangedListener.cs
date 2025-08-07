using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;
using UserService.Infrastructure.MSSQL;
using UserService.Infrastructure.RabbitMQService.Consumers;

namespace UserService.Infrastructure.BackgroundJobs;

public class DayResultWeightChangedListener : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DayResultWeightChangedConsumer _consumer;

    public DayResultWeightChangedListener(IServiceScopeFactory scopeFactory, DayResultWeightChangedConsumer consumer)
    {
        _scopeFactory = scopeFactory;
        _consumer = consumer;
    }

    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _consumer.AddListenerAsync(async args =>
        {
            var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var data = JsonConvert.DeserializeObject<WeightChangedMessage>(message);

                if (data != null)
                {
                    var profile = await dbContext.Profiles.Where(p => p.Id == data.ProfileId).FirstOrDefaultAsync(cancellationToken);

                    if (profile != null) 
                    {
                        profile.Weight = data.NewWeight;

                        dbContext.Update(profile);

                        await dbContext.SaveChangesAsync(cancellationToken);
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"{DateTime.Now} [ERROR] Failed to process message: {ex.Message}");
            }
        },
        cancellationToken: cancellationToken);
    }

    private class WeightChangedMessage
    {
        public Guid ProfileId { get; set; }
        public double NewWeight { get; set; }
    }
}
