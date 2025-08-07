using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using UserService.Infrastructure.MSSQL;
using Microsoft.EntityFrameworkCore;
using UserService.Contracts.Exceptions;
using UserService.Infrastructure.RabbitMQService.Consumers;

namespace UserService.Infrastructure.BackgroundJobs;

public class RevokeMealPlanListener : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMQRevokeMealPlanConsumer _consumer;

    public RevokeMealPlanListener(IServiceScopeFactory scopeFactory, RabbitMQRevokeMealPlanConsumer consumer)
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

                    profile.ThereIsMealPlan = false;

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
