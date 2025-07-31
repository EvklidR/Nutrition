
using FoodService.Api.Middleware;
using FoodService.API.DependencyInjection;
using FoodService.API.Extentions;
using FoodService.Application.DependencyInjection;
using FoodService.Infrastructure.DependencyInjection;
using Hangfire;

namespace FoodService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddApiServices(builder, builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        var app = builder.Build();

        app.UseCors("AllowSpecificOrigin");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations();
        }

        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            app.UseHangfireDashboard();
            scope.AddJobs();
        }


        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
