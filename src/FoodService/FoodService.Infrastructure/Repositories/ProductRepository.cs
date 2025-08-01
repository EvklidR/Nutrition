using Microsoft.EntityFrameworkCore;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;
using FoodService.Domain.Repositories.Models;
using FoodService.Infrastructure.Extentions;

namespace FoodService.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(IEnumerable<Product>?, long)> GetAllAsync(Guid userId, GetFoodRequestParameters parameters)
    {
        var query = _dbSet
            .Where(d => d.UserId == userId)
            .GetByName(parameters.Name)
            .SortByCriteria(parameters.SortAsc, parameters.SortingCriteria);

        long totalRecords = await query.CountAsync();

        var products = await query
            .GetPaginated(parameters.PaginationParameters)
            .Cast<Product>()
            .ToListAsync();

        return (products, totalRecords);
    }
}
