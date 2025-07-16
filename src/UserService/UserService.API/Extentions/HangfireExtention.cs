using Hangfire;

namespace UserService.API.Extentions;

public static class HangfireExtention
{
    public static void SetHangfireConfiguration(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        var connectionString = scope.ServiceProvider.GetRequiredService<IConfiguration>()
            .GetConnectionString("DefaultConnection");

        GlobalConfiguration.Configuration
            .UseSqlServerStorage(connectionString);
    }
}
