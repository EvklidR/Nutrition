using PostService.API.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace PostService.API.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services,
            WebApplicationBuilder builder,
            IConfiguration configuration)
        {
            services.AddScoped<UserIdFilter>();

            var allowedOrigins = configuration.GetSection("AllowedOrigins:CorsOrigins").Get<string[]>();
            if (allowedOrigins != null && allowedOrigins.Length > 0)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowSpecificOrigin", builder =>
                    {
                        builder.WithOrigins(allowedOrigins)
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                    });
                });
            }

            var log = new LoggerConfiguration()
                 .WriteTo.Http("http://localhost:8080", null) // It will be port of logstash later
                 .CreateLogger();

            builder.Host.UseSerilog();

            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Post Service API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token as: Bearer {token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddAuthorization();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["AuthOptions:Issuer"],
                        ValidAudience = configuration["AuthOptions:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["AuthOptions:Key"])
                        )
                    };
                });

            return services;
        }
    }
}