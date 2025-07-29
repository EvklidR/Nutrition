using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.RabbitMQService;
using UserService.Infrastructure.MSSQL;
using UserService.Contracts.Exceptions;

namespace UserService.Infrastructure.BackgroundJobs;

public class ChooseMealPlanService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMQChooseMealPlanConsumer _consumer;

    public ChooseMealPlanService(IServiceScopeFactory scopeFactory, RabbitMQChooseMealPlanConsumer consumer)
    {
        _scopeFactory = scopeFactory;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _consumer.AddListenerAsync(async args =>
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                if (Guid.TryParse(message, out var profileId))
                {
                    var profile = await dbContext.Profiles.FirstOrDefaultAsync(
                        p => p.Id == profileId,
                        cancellationToken);

                    if (profile == null)
                    {
                        throw new NotFound("Profile not found");
                    }

                    profile.ThereIsMealPlan = true;

                    dbContext.Profiles.Update(profile);
                    await dbContext.SaveChangesAsync(cancellationToken);
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
        },
        cancellationToken);
    }
}
