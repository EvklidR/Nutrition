using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Infrastructure.MSSQL;
using UserService.Infrastructure.Repositories;
using UserService.Domain.Interfaces.Repositories;
using UserService.Infrastructure.Services;
using UserService.Infrastructure.Configurations;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Grpc;
using Hangfire;

namespace UserService.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpc();

            services.AddSingleton<IMealPlanService>(sp =>
                new MealPlanServiceClient(configuration["GrpcServices:MealPlanServiceUrl"]));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfire(conf => conf
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfireServer();

            services.AddHttpContextAccessor();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IRefreshTokenTokenRepository, RefreshTokenTokenRepository>();

            services.AddScoped<RoleInitializer>();

            return services;
        }
    }
}
