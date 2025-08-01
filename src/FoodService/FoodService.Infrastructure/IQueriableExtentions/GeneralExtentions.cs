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

        if (startDate != null && endDate != null)
        {
            collection = collection.Where(dr => dr.Date >= startDate && dr.Date <= endDate);
        }

        return collection;
    }

    public static IQueryable<T> GetPaginated<T>(this IQueryable<T> collection, PaginatedParameters? paginatedParameters)
    {
        if (paginatedParameters == null)
        {
            return collection;
        }

        var page = paginatedParameters.Page;
        var pageSize = paginatedParameters.PageSize;

        if (page != null && pageSize != null)
        {
            collection = collection.Skip((int)pageSize * ((int)page - 1)).Take((int)pageSize);
        }

        return collection;
    }
}
