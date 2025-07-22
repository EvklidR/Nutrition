using MongoDB.Driver;

namespace PostService.Infrastructure.Extentions.IFindFluentExtensions
{
    public static class IFindFluentExtensions
    {
        public static IFindFluent<D,P> GetPaginated<D, P>(this IFindFluent<D, P> list, int? page, int? size)
        {
            if (page.HasValue && size.HasValue)
            {
                return list.Skip((page.Value - 1) * size.Value).Limit(size.Value);
            }

            return list;
        }
    }
}
