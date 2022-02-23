FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["TobyMeehan.Com/TobyMeehan.Com.csproj", "TobyMeehan.Com/"]
COPY ["DataAccessLibrary/DataAccessLibrary.csproj", "DataAccessLibrary/"]
COPY ["TobyMeehan.Com.AspNetCore/TobyMeehan.Com.AspNetCore.csproj", "TobyMeehan.Com.AspNetCore/"]
RUN dotnet restore "TobyMeehan.Com/TobyMeehan.Com.csproj"
COPY . .
WORKDIR "/src/TobyMeehan.Com"
RUN dotnet build "TobyMeehan.Com.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TobyMeehan.Com.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TobyMeehan.Com.dll"]
