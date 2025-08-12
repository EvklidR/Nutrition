using FoodService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FoodService.Infrastructure.MSSQL.Configuraions
{
    public class EatenDishConfiguration : IEntityTypeConfiguration<EatenDish>
    {
        public void Configure(EntityTypeBuilder<EatenDish> builder)
        {
            builder.HasIndex(ed => new { ed.DishId, ed.MealId }).IsUnique();

            builder.HasOne<Meal>()
                .WithMany(m => m.Dishes)
                .HasForeignKey(ef => ef.MealId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ep => ep.Dish)
                .WithMany()
                .HasForeignKey(ep => ep.DishId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
