using FoodService.Application.Exceptions;
using FoodService.Domain.Entities;
using FoodService.Domain.Repositories.Models;
using System.Linq.Expressions;

namespace FoodService.Infrastructure.Extentions
{
    public static class FoodLINQExtentions
    {
        public static IQueryable<Food> GetByName(this IQueryable<Food> collection, string? Name)
        {
            if (Name != null)
            {
                collection = collection.Where(x => x.Name.Contains(Name));
            }

            return collection;
        }

        public static IQueryable<Food> GetPaginated(this IQueryable<Food> collection, int? page, int? size)
        {
            if (page != null && size != null)
            {
                collection = collection.Skip((int)size * ((int)page - 1)).Take((int)size);
            }

            return collection;
        }

        public static IQueryable<Food> SortByCriteria(
            this IQueryable<Food> collection,
            bool? sortAsc,
            SortingCriteria criteria)
        {
            if (sortAsc == null)
            {
                return collection;
            }

            var keySelector = GetSortingExpression(criteria);

            return sortAsc == true
                ? collection.OrderBy(keySelector)
                : collection.OrderByDescending(keySelector);
        }

        private static Expression<Func<Food, double>> GetSortingExpression(SortingCriteria criteria)
        {
            return criteria switch
            {
                SortingCriteria.Calories => x => x.Calories,
                SortingCriteria.Proteins => x => x.Proteins,
                SortingCriteria.Fats => x => x.Fats,
                SortingCriteria.Carbohydrates => x => x.Carbohydrates,
                _ => throw new BadRequest("Invalid criteria")
            };
        }
    }
}
