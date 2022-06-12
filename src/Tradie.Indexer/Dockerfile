#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tradie.Indexer/Tradie.Indexer.csproj", "Tradie.Indexer/"]
RUN dotnet restore "Tradie.Indexer/Tradie.Indexer.csproj"
COPY . .
WORKDIR "/src/Tradie.Indexer"
RUN dotnet build "Tradie.Indexer.csproj" --no-self-contained -r linux-x64 -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tradie.Indexer.csproj" --no-self-contained -r linux-x64 -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tradie.Indexer.dll"]
