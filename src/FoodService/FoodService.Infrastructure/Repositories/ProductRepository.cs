using Microsoft.EntityFrameworkCore;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;
using FoodService.Domain.Repositories.Models;
using FoodService.Infrastructure.Extentions;

namespace FoodService.Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>?> GetAllAsync(Guid userId, GetFoodRequestParameters parameters)
        {
            return (IEnumerable<Product>?)await _dbSet
                .GetByName(parameters.Name)
                .SortByCriteria(parameters.SortAsc, parameters.SortingCriteria)
                .GetPaginated(parameters.Page, parameters.PageSize)
                .Cast<Product>()
                .ToListAsync();
        }
    }
}
