using FoodService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodService.Infrastructure.MSSQL.Configuraions;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasOne(r => r.Dish)
            .WithOne(d => d.Recipe)
            .HasForeignKey<Recipe>(r => r.DishId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
