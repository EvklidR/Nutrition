using UserService.Application.DependencyInjection;
using UserService.API.Middleware;
using UserService.API.DependencyInjection;
using UserService.Infrastructure.DependencyInjection;
using UserService.API.Extentions;
using Hangfire;

namespace UserService.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddApiServices(builder, builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        var app = builder.Build();

        app.MapGrpcService<Infrastructure.gRPC.UserService>();

        app.UseCors("AllowSpecificOrigin");

        app.MapGrpcService<Infrastructure.gRPC.UserService>();

        app.UseHangfireDashboard();

        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        await app.InitializeAdmin();

        app.MapControllers();
        app.MapHangfireDashboard();

        app.Run();
    }
}
