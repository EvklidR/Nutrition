using FoodService.Infrastructure.MSSQL;
using Microsoft.EntityFrameworkCore;

namespace FoodService.Infrastructure.BackgroundJobs;

public class CreateTodayDayResultJob
{
    private readonly ApplicationDbContext _dbContext;

    public CreateTodayDayResultJob(ApplicationDbContext dbContext)
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


    }
}
