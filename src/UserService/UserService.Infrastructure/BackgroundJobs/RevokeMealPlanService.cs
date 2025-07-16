using Microsoft.Extensions.Hosting;
using UserService.Infrastructure.RabbitMQService;
using System.Text;
using UserService.Infrastructure.MSSQL;
using Microsoft.EntityFrameworkCore;
using UserService.Contracts.Exceptions;

namespace UserService.Infrastructure.BackgroundJobs;

public class RevokeMealPlanService : BackgroundService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RabbitMQRevokeMealPlanConsumer _consumer;

    public RevokeMealPlanService(ApplicationDbContext dbContext, RabbitMQRevokeMealPlanConsumer consumer)
    {
        _dbContext = dbContext;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _consumer.AddListenerAsync(async args =>
        {
            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                if (Guid.TryParse(message, out var profileId))
                {
                    var profile = await _dbContext.Profiles.FirstOrDefaultAsync(profile => profile.Id == profileId, cancellationToken);

                    if (profile == null)
                    {
                        throw new NotFound("Profile not found");
                    }

                    profile.ThereIsMealPlan = false;

                    _dbContext.Profiles.Update(profile);

                    await _dbContext.SaveChangesAsync(cancellationToken);
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
