using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MealPlanService.Infrastructure.MongoDB;
using MealPlanService.Infrastructure.Repositories;
using MealPlanService.Infrastructure.Options;
using MealPlanService.Infrastructure.Services;

namespace MealPlanService.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(sp =>
                new UserService(configuration["GrpcServices:UserServiceUrl"]));

            services.Configure<MongoDBOptions>(configuration.GetSection("ConnectionStrings"));
            services.AddScoped<MongoDBContext>();

            services.AddScoped<MealPlanRepository>();
            services.AddScoped<ProfileMealPlanRepository>();

            return services;
        }
    }
}
