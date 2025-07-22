namespace PostService.Infrastructure.Extentions.IQueriableExtensions
{
    public static class PaginationExtention
    {
        public static IQueryable<T> GetPaginated<T>(this IQueryable<T> list, int? page, int? size)
        {
            if (page.HasValue && size.HasValue)
            {
                return list.Skip((page.Value - 1) * size.Value).Take(size.Value);
            }

            return list;
        }
    }
}
