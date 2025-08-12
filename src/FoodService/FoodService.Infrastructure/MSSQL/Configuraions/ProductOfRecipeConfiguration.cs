using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodService.Domain.Entities;

namespace FoodService.Infrastructure.MSSQL.Configurations
{
    public class ProductOfRecipeConfiguration : IEntityTypeConfiguration<ProductOfRecipe>
    {
        public void Configure(EntityTypeBuilder<ProductOfRecipe> builder)
        {
            builder.HasKey(iod => new 
            {
                iod.RecipeId,
                iod.ProductId 
            });

            builder.HasOne<Recipe>()
                .WithMany(r => r.Ingredients)
                .HasForeignKey(por => por.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(por => por.Product)
                .WithMany()
                .HasForeignKey(por => por.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
