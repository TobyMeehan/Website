FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/TobyMeehan.Com.Accounts/TobyMeehan.Com.Accounts.csproj", "src/TobyMeehan.Com.Accounts/"]
RUN dotnet restore "src/TobyMeehan.Com.Accounts/TobyMeehan.Com.Accounts.csproj"
COPY . .
WORKDIR "/src/src/TobyMeehan.Com.Accounts"
RUN dotnet build "TobyMeehan.Com.Accounts.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TobyMeehan.Com.Accounts.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TobyMeehan.Com.Accounts.dll"]
