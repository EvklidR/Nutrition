using Microsoft.Extensions.DependencyInjection;
using PostService.Infrastructure.MongoDB;
using PostService.Infrastructure.Repositories;
using PostService.Infrastructure.Options;
using PostService.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using PostService.Infrastructure.gRPC.Interfaces;
using PostService.Infrastructure.gRPC;
using PostService.Infrastructure.Services.Interfaces;
using PostService.Infrastructure.Services;

namespace PostService.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDBOptions>(configuration.GetSection("ConnectionStrings"));
            services.AddScoped<MongoDBContext>();

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();

            services.AddScoped<IUserService>(provider => new UserService(configuration["GrpcServices:UserServiceUrl"]));

            var appKey = configuration["DropBox:AppKey"];
            var appSecret = configuration["DropBox:AppSecret"];
            var refresh = configuration["DropBox:RefreshToken"];
            services.AddScoped<IImageService>(provider => new ImageService(appKey, appSecret, refresh));

            return services;
        }
    }
}