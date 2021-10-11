#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Tradie.Scanner/Tradie.Scanner.csproj", "Tradie.Scanner/"]
RUN dotnet restore "Tradie.Scanner/Tradie.Scanner.csproj"
COPY . .
WORKDIR "/src/Tradie.Scanner"
RUN dotnet build "Tradie.Scanner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tradie.Scanner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tradie.Scanner.dll"]