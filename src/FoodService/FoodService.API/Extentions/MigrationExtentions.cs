using Microsoft.EntityFrameworkCore;
using FoodService.Infrastructure.MSSQL;

namespace FoodService.API.Extentions
{
    public static class MigrationExtentions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();
        }
    }
}
