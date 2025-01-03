using UserService.Infrastructure.Configurations;

namespace UserService.API.Extentions
{
    public static class InitializationExtension
    {
        public async static Task InitializeAdmin(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            RoleInitializer initializer = scope.ServiceProvider.GetRequiredService<RoleInitializer>();

            await initializer.InitializeAsync();
        }
    }
}
