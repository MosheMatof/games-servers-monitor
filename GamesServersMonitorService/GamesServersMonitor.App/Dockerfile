#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GamesServersMonitor.App/GamesServersMonitor.App.csproj", "GamesServersMonitor.App/"]
COPY ["GamesServersMonitor.Domain/GamesServersMonitor.Domain.csproj", "GamesServersMonitor.Domain/"]
COPY ["GamesServersMonitor.Infrastructure/GamesServersMonitor.Infrastructure.csproj", "GamesServersMonitor.Infrastructure/"]
RUN dotnet restore "GamesServersMonitor.App/GamesServersMonitor.App.csproj"
COPY . .
WORKDIR "/src/GamesServersMonitor.App"
RUN dotnet build "GamesServersMonitor.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GamesServersMonitor.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GamesServersMonitor.App.dll"]