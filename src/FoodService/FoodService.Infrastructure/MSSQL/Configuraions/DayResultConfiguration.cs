using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodService.Domain.Entities;

namespace FoodService.Infrastructure.MSSQL.Configurations
{
    public class DayResultConfiguration : IEntityTypeConfiguration<DayResult>
    {
        public void Configure(EntityTypeBuilder<DayResult> builder)
        {
            builder.HasMany(dr => dr.Meals)
                .WithOne()
                .HasForeignKey(m => m.DayId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(dr => new { dr.ProfileId, dr.Date });
        }
    }
}
