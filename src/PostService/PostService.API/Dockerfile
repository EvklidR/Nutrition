# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
EXPOSE 7063

COPY ["src/PostService/PostService.API/certs/https-cert.crt", "/usr/local/share/ca-certificates/https-cert.crt"]
RUN update-ca-certificates  

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/PostService/PostService.API/PostService.API.csproj", "src/PostService/PostService.API/"]
COPY ["src/PostService/PostService.BusinessLogic/PostService.BusinessLogic.csproj", "src/PostService/PostService.BusinessLogic/"]
COPY ["src/PostService/PostService.Infrastructure/PostService.Infrastructure.csproj", "src/PostService/PostService.Infrastructure/"]
COPY ["src/PostService/PostService.Core/PostService.Core.csproj", "src/PostService/PostService.Core/"]
RUN dotnet restore "./src/PostService/PostService.API/PostService.API.csproj"
COPY . .
WORKDIR "/src/src/PostService/PostService.API"
RUN dotnet build "./PostService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PostService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostService.API.dll"]

