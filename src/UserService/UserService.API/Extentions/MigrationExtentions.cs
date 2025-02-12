using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserService.Infrastructure.MSSQL;

namespace UserService.API.Extentions
{
    public static class MigrationExtentions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();

            var connectionString = scope.ServiceProvider.GetRequiredService<IConfiguration>()
                .GetConnectionString("DefaultConnection");

            // Настроим Hangfire с использованием SQL Server
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(connectionString);
        }
    }
}
