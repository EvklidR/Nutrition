# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
EXPOSE 7022

COPY ["src/FoodService/FoodService.API/certs/https-cert.crt", "/usr/local/share/ca-certificates/nginx.crt"]
RUN update-ca-certificates 

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FoodService/FoodService.API/FoodService.API.csproj", "src/FoodService/FoodService.API/"]
COPY ["src/Nutrition.ServiceDefaults/Nutrition.ServiceDefaults.csproj", "src/Nutrition.ServiceDefaults/"]
COPY ["src/FoodService/FoodService.Application/FoodService.Application.csproj", "src/FoodService/FoodService.Application/"]
COPY ["src/FoodService/FoodService.Domain/FoodService.Domain.csproj", "src/FoodService/FoodService.Domain/"]
COPY ["src/FoodService/FoodService.Infrastructure/FoodService.Infrastructure.csproj", "src/FoodService/FoodService.Infrastructure/"]
RUN dotnet restore "./src/FoodService/FoodService.API/FoodService.API.csproj"
COPY . .
WORKDIR "/src/src/FoodService/FoodService.API"
RUN dotnet build "./FoodService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FoodService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodService.API.dll"]