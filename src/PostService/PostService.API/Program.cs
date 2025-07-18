using PostService.API.DependencyInjection;
using PostService.API.Middleware;
using PostService.BusinessLogic.DependencyInjection;
using PostService.Infrastructure.DependencyInjection;

namespace PostService.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddApiServices(builder, builder.Configuration);

        var app = builder.Build();

        app.UseCors("AllowSpecificOrigin");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
