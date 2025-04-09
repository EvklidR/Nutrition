using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.MSSQL.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(p => p.UserId) 
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
