using FoodService.Application.Interfaces;
using FoodService.Domain.Interfaces;
using FoodService.Domain.Interfaces.Repositories;
using FoodService.Infrastructure.BackgroundJobs;
using FoodService.Infrastructure.Grpc;
using FoodService.Infrastructure.MSSQL;
using FoodService.Infrastructure.RabbitMQService;
using FoodService.Infrastructure.RabbitMQService.Consumers;
using FoodService.Infrastructure.RabbitMQService.Listeners;
using FoodService.Infrastructure.RabbitMQService.Settings;
using FoodService.Infrastructure.Repositories;
using FoodService.Infrastructure.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace FoodService.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService>(provider =>
            {
                var url = configuration["GrpcServices:UserServiceUrl"];
                var cacheService = provider.GetRequiredService<ICacheService>();

                return new UserService(url, cacheService);
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfire(conf => conf
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfireServer();

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

            services.AddScoped<CreateDayResultsJob>();

            var rabbitMqSection = configuration.GetSection("RabbitMq");
            services.Configure<RabbitMqSettings>(rabbitMqSection);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value);

            services.AddSingleton<IBrokerService, RabbitMQProducer>();
            services.AddSingleton<ProfileDeletedConsumer>();
            services.AddSingleton<ProfileCreatedConsumer>();
            services.AddSingleton<ChangeProfileWeightConsumer>();
            services.AddHostedService<ChangeProfileWeightListener>();
            services.AddHostedService<ProfileCreatedListener>();
            services.AddHostedService<ProfileDeletedListener>();

            return services;
        }
    }
}
