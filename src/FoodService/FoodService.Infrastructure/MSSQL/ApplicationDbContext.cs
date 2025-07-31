using Microsoft.EntityFrameworkCore;
using FoodService.Domain.Entities;

namespace FoodService.Infrastructure.MSSQL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<DayResult> DayResults { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOfRecipe> ProductsOfRecipes { get; set; }
        public DbSet<EatenDish> EatenDishes { get; set; }
        public DbSet<EatenProduct> EatenProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
