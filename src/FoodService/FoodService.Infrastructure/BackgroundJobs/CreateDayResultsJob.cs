using FoodService.Domain.Entities;
using FoodService.Infrastructure.MSSQL;
using Microsoft.EntityFrameworkCore;

namespace FoodService.Infrastructure.BackgroundJobs;

public class CreateDayResultsJob
{
    private readonly ApplicationDbContext _dbContext;

    public CreateDayResultsJob(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Run()
    {
        var lastDayResults = await _dbContext.DayResults
            .GroupBy(dr => dr.ProfileId)
            .Select(g => g
                .OrderByDescending(a => a.Date)
                .FirstOrDefault())
            .Where(dr => dr != null)
            .ToListAsync();

        var now = DateOnly.FromDateTime(DateTime.Now);
        var dayResultsToAdd = new List<DayResult>();

        foreach (var dayResult in lastDayResults)
        {
            if (dayResult!.Date != now) {
                var day = dayResult.Date.AddDays(1);

                while (day <= now)
                {
                    dayResultsToAdd.Add(new DayResult
                    {
                        Date = day,
                        ProfileId = dayResult.ProfileId,
                        GlassesOfWater = 0,
                        Weight = dayResult.Weight
                    });
                    day.AddDays(1);
                }
            }
        }

        _dbContext.AddRange(dayResultsToAdd);

        await _dbContext.SaveChangesAsync();
    }
}
