#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM public.ecr.aws/w1h1o7p8/dotnet6:latest AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tradie.Scanner/Tradie.Scanner.csproj", "Tradie.Scanner/"]
RUN dotnet restore "Tradie.Scanner/Tradie.Scanner.csproj"
COPY . .
WORKDIR "/src/Tradie.Scanner"
RUN dotnet build "Tradie.Scanner.csproj" --self-contained -r linux-arm64 -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tradie.Scanner.csproj" --self-contained -r linux-arm64 -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["Tradie.Analyzer::Tradie.Analyzer.Function::FunctionHandler"]
