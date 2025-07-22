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
            services.AddScoped<ICheckUserService>(provider =>
            {
                var url = configuration["GrpcServices:UserServiceUrl"];
                var cacheService = provider.GetRequiredService<ICacheService>();

                return new CheckUserService(url, cacheService);
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IDayResultRepository, DayResultRepository>();
            services.AddScoped<IDishRepository, DishRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ISearchProductService, SearchProductService>();

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration["Redis:Url"]));
            services.AddScoped<ICacheService, RedisCacheService>();

            services.AddScoped<IImageService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var appKey = configuration["DropBox:AppKey"];
                var appSecret = configuration["DropBox:AppSecret"];
                var refresh = configuration["DropBox:RefreshToken"];
                var cacheService = provider.GetRequiredService<ICacheService>();

                return new ImageService(appKey, appSecret, refresh, cacheService);
            });

            return services;
        }
    }
}
