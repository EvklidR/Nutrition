using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using PostService.BusinessLogic.Services;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;


namespace PostService.BusinessLogic.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<Services.PostService>();
            services.AddScoped<CommentService>();

            return services;

        }
    }
}