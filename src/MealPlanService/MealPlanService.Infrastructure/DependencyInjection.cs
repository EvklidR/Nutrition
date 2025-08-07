using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MealPlanService.Infrastructure.MongoDB;
using MealPlanService.Infrastructure.Repositories;
using MealPlanService.Infrastructure.Options;
using MealPlanService.Infrastructure.Services;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MealPlanService.Infrastructure.Services.Interfaces;
using MealPlanService.Infrastructure.RabbitMQService;
using Microsoft.Extensions.Options;
using MealPlanService.Infrastructure.RabbitMQService.Settings;

namespace MealPlanService.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService>(provider => new UserService(configuration["GrpcServices:UserServiceUrl"]));

            services.Configure<MongoDBOptions>(configuration.GetSection("ConnectionStrings"));
            services.AddScoped<MongoDBContext>();

            services.AddScoped<IMealPlanRepository, MealPlanRepository>();
            services.AddScoped<IProfileMealPlanRepository, ProfileMealPlanRepository>();

            var rabbitMqSection = configuration.GetSection("RabbitMq");
            services.Configure<RabbitMqSettings>(rabbitMqSection);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value);

            services.AddSingleton<DeleteProfileConsumer>();
            services.AddSingleton<IBrokerService, RabbitMQProducer>();

            return services;
        }
    }
}
