using FoodService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FoodService.Infrastructure.MSSQL.Configuraions
{
    public class EatenProductConfiguration : IEntityTypeConfiguration<EatenProduct>
    {
        public void Configure(EntityTypeBuilder<EatenProduct> builder)
        {
            builder.HasIndex(ep => new { ep.ProductId, ep.MealId }).IsUnique();

            builder.HasOne<Meal>()
                .WithMany(m => m.Products)
                .HasForeignKey(ef => ef.MealId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ep => ep.Product)
                .WithMany()
                .HasForeignKey(ep => ep.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
