namespace PostService.Infrastructure.Extentions
{
    public static class PaginationExtention
    {
        public static IEnumerable<T> GetPaginated<T>(this IEnumerable<T> list, int? page, int? size)
        {
            if (page.HasValue && size.HasValue)
            {
                return list.Skip((page.Value - 1) * size.Value).Take(size.Value);
            }

            return list;
        }
    }
}
