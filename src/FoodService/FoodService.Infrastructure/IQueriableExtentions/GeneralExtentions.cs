using FoodService.Domain.Entities.Interfaces;
using FoodService.Domain.Interfaces.Repositories.Models;

namespace FoodService.Infrastructure.IQueriableExtentions;

public static class GeneralExtentions
{
    public static IQueryable<T> GetByPeriod<T>(this IQueryable<T> collection, PeriodParameters? periodParameters) where T : IHasDate
    {
        if (periodParameters == null)
        {
            return collection;
        }

        var startDate = periodParameters.StartDate;
        var endDate = periodParameters.EndDate;

        collection = collection.Where(dr => dr.Date >= startDate && dr.Date <= endDate);

        return collection;
    }

    public static IQueryable<T> GetPaginated<T>(this IQueryable<T> collection, PaginationParameters? paginationParameters)
    {
        if (paginationParameters == null || paginationParameters.Page == 0 || paginationParameters.PageSize == 0)
        {
            return collection;
        }

        var page = paginationParameters.Page;
        var pageSize = paginationParameters.PageSize;

        collection = collection.Skip((int)pageSize * ((int)page - 1)).Take((int)pageSize);

        return collection;
    }
}
