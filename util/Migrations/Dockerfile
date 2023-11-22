FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["util/Migrations/Migrations.csproj", "util/Migrations/"]
RUN dotnet restore "util/Migrations/Migrations.csproj"
COPY . .
WORKDIR "/src/util/Migrations"
RUN dotnet build "Migrations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Migrations.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Migrations.dll"]
