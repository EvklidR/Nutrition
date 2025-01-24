using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FoodService.Infrastructure.MSSQL;
using FoodService.Infrastructure.Repositories;
using FoodService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Application.Interfaces;
using FoodService.Infrastructure.Services;
using StackExchange.Redis;
using FoodService.Infrastructure.Grpc;

namespace FoodService.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IUserService>(sp =>
                new UserService(configuration["GrpcServices:UserServiceUrl"]));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IDayResultRepository, DayResultRepository>();
            services.AddScoped<IDishRepository, DishRepository>();
            services.AddScoped<IIngredientRepository, IngredientRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ISearchProductService, SearchProductService>();

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), configuration["ImageSettings:ImagePath"]);
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("127.0.0.1:6379"));

            return services;
        }
    }
}
