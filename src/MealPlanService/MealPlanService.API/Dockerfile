# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 7132


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR .
COPY ["src/MealPlanService/MealPlanService.API/MealPlanService.API.csproj", "src/MealPlanService/MealPlanService.API/"]
COPY ["src/Nutrition.ServiceDefaults/Nutrition.ServiceDefaults.csproj", "src/Nutrition.ServiceDefaults/"]
COPY ["src/MealPlanService/MealPlanService.BusinessLogic/MealPlanService.BusinessLogic.csproj", "src/MealPlanService/MealPlanService.BusinessLogic/"]
COPY ["src/MealPlanService/MealPlanService.Infrastructure/MealPlanService.Infrastructure.csproj", "src/MealPlanService/MealPlanService.Infrastructure/"]
COPY ["src/MealPlanService/MealPlanService.Core/MealPlanService.Core.csproj", "src/MealPlanService/MealPlanService.Core/"]
RUN dotnet restore "./src/MealPlanService/MealPlanService.API/MealPlanService.API.csproj"
COPY . .
WORKDIR "/src/MealPlanService/MealPlanService.API"
RUN dotnet build "./MealPlanService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MealPlanService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MealPlanService.API.dll"]