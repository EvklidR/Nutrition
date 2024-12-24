var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.UserService_API>("userservice-api");

builder.AddProject<Projects.FoodService_API>("foodservice-api");

builder.AddProject<Projects.MealPlanService_API>("mealplanservice-api");

builder.AddProject<Projects.PostService_API>("postservice-api");

builder.Build().Run();
