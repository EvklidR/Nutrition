using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodService.Domain.Entities;

namespace FoodService.Infrastructure.MSSQL.Configurations
{
    public class EatenFoodConfiguration : IEntityTypeConfiguration<EatenFood>
    {
        public void Configure(EntityTypeBuilder<EatenFood> builder)
        {
            builder.HasKey(ef => new { ef.FoodId, ef.MealId });

            builder.HasOne(ef => ef.Food)
                .WithMany()
                .HasForeignKey(ef => ef.FoodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
