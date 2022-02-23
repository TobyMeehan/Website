FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["TobyMeehan.Com.Api/TobyMeehan.Com.Api.csproj", "TobyMeehan.Com.Api/"]
COPY ["DataAccessLibrary/DataAccessLibrary.csproj", "DataAccessLibrary/"]
COPY ["TobyMeehan.Com.AspNetCore/TobyMeehan.Com.AspNetCore.csproj", "TobyMeehan.Com.AspNetCore/"]
RUN dotnet restore "TobyMeehan.Com.Api/TobyMeehan.Com.Api.csproj"
COPY . .
WORKDIR "/src/TobyMeehan.Com.Api"
RUN dotnet build "TobyMeehan.Com.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TobyMeehan.Com.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TobyMeehan.Com.Api.dll"]
