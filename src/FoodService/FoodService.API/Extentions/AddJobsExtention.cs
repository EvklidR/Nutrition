using FoodService.Infrastructure.BackgroundJobs;
using Hangfire;

namespace FoodService.API.Extentions;

public static class AddJobsExtention
{
    public static IServiceScope AddJobs(this IServiceScope services)
    {
        BackgroundJob.Enqueue<CreateDayResultsJob>(job => job.Run());

        RecurringJob.AddOrUpdate<CreateDayResultsJob>("CreateDayResults",
            job => job.Run(),
            Cron.Daily(0));

        return services;
    }
}
