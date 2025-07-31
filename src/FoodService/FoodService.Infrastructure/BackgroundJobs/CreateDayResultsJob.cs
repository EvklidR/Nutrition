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
            .Join(
                _dbContext.DayResults
                    .GroupBy(dr => dr.ProfileId)
                    .Select(g => new
                    {
                        ProfileId = g.Key,
                        LastDate = g.Max(x => x.Date)
                    }),
                dr => new { dr.ProfileId, dr.Date },
                g => new { g.ProfileId, Date = g.LastDate },
                (dr, g) => dr
            )
            .ToListAsync();

        var now = DateOnly.FromDateTime(DateTime.Now);
        var dayResultsToAdd = new List<DayResult>();

        foreach (var dayResult in lastDayResults)
        {
            if (dayResult!.Date != now) 
            {
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
                    day = day.AddDays(1);
                }
            }
        }

        _dbContext.AddRange(dayResultsToAdd);

        await _dbContext.SaveChangesAsync();
    }
}
