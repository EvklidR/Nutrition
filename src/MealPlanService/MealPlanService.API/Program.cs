using MealPlanService.API.DependencyInjection;
using MealPlanService.API.Middleware;
using MealPlanService.Infrastructure.DependencyInjection;
using UserProfileService.BusinessLogic.DependencyInjection;

namespace MealPlanService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        builder.Services.AddApiServices(builder, builder.Configuration);

        var app = builder.Build();

        app.UseCors("AllowSpecificOrigin");

        app.MapGrpcService<gRPC.MealPlanService>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
