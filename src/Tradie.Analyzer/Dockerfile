#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM 818120129739.dkr.ecr.us-east-1.amazonaws.com/dotnet6:latest AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tradie.Analyzer/Tradie.Analyzer.csproj", "Tradie.Analyzer/"]
RUN dotnet restore "Tradie.Analyzer/Tradie.Analyzer.csproj"
COPY . .
WORKDIR "/src/Tradie.Analyzer"
RUN dotnet build "Tradie.Analyzer.csproj" --self-contained -r linux-arm64 -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tradie.Analyzer.csproj" --self-contained -r linux-arm64 -c Release -o /app/publish

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["Tradie.Analyzer::Tradie.Analyzer.Function::FunctionHandler"]
