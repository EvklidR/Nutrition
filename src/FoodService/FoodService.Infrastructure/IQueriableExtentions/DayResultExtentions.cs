using FoodService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodService.Infrastructure.IQueriableExtentions;

public static class DayResultExtentions
{
    public static IQueryable<DayResult> IncludeFood(this IQueryable<DayResult> collection)
    {
        return collection
            .Include(dr => dr.Meals)
                .ThenInclude(m => m.Dishes)
                    .ThenInclude(f => f.Food)
            .Include(d => d.Meals)
                .ThenInclude(m => m.Products)
                    .ThenInclude(p => p.Food);
    }
}
