FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tradie.Migrations/Tradie.Migrations.csproj", "Tradie.Migrations/"]
RUN dotnet restore "Tradie.Migrations/Tradie.Migrations.csproj"
COPY . .
WORKDIR "/src/Tradie.Migrations"
RUN dotnet build "Tradie.Migrations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tradie.Migrations.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tradie.Migrations.dll"]
