using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.MSSQL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Profile> Profiles { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}