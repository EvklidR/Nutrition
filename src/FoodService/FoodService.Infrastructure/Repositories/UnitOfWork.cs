using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;
using FoodService.Domain.Interfaces;

namespace FoodService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IDishRepository? _dishRepository;
        private IProductRepository? _productRepository;
        private IDayResultRepository? _dayResultRepository;
        private IRecipeRepository? _recipeRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IDishRepository DishRepository
            => _dishRepository ??= new DishRepository(_context);

        public IProductRepository ProductRepository
            => _productRepository ??= new ProductRepository(_context);

        public IDayResultRepository DayResultRepository
            => _dayResultRepository ??= new DayResultRepository(_context);

        public IRecipeRepository RecipeRepository
            => _recipeRepository ??= new RecipeRepository(_context);

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
