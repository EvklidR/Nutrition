using Microsoft.EntityFrameworkCore;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.MSSQL;

namespace FoodService.Infrastructure.Repositories
{
    public class IngredientRepository : BaseRepository<Ingredient>, IIngredientRepository
    {
        public IngredientRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
