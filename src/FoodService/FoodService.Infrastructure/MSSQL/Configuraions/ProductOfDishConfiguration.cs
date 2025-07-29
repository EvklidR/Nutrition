using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodService.Domain.Entities;

namespace FoodService.Infrastructure.MSSQL.Configurations
{
    public class ProductOfDishConfiguration : IEntityTypeConfiguration<ProductOfRecipe>
    {
        public void Configure(EntityTypeBuilder<ProductOfRecipe> builder)
        {
            builder.HasKey(iod => new 
            {
                iod.RecipeId,
                iod.ProductId 
            });

            builder.HasOne<Recipe>()
                .WithMany(d => d.Ingredients)
                .HasForeignKey(iod => iod.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(iod => iod.Product)
                .WithMany()
                .HasForeignKey(iod => iod.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
