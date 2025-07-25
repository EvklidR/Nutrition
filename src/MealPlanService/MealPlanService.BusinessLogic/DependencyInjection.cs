﻿using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using MealPlanService.BusinessLogic.Services;
using MealPlanService.Infrastructure.Services.Interfaces;
using MealPlanService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;


namespace UserProfileService.BusinessLogic.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<MealPlanService.BusinessLogic.Services.MealPlanService>();
            services.AddScoped<ProfilePlanService>();

            return services;

        }
    }
}
