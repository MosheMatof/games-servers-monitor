#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IGDB.Api/IGDB.Api.csproj", "IGDB.Api/"]
COPY ["IGDB.Domain/IGDB.Domain.csproj", "IGDB.Domain/"]
COPY ["IGDB.Infrastructure/IGDB.Infrastructure.csproj", "IGDB.Infrastructure/"]
RUN dotnet restore "IGDB.Api/IGDB.Api.csproj"
COPY . .
WORKDIR "/src/IGDB.Api"
RUN dotnet build "IGDB.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IGDB.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IGDB.Api.dll"]