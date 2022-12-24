#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM 818120129739.dkr.ecr.us-east-1.amazonaws.com/dotnet6:latest AS base
#FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
FROM public.ecr.aws/lambda/dotnet:6 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tradie.Analyzer/Tradie.ElasticIndexer.csproj", "Tradie.ElasticIndexer/"]
RUN dotnet restore "Tradie.Analyzer/Tradie.ElasticIndexer.csproj"
COPY . .
WORKDIR "/src/Tradie.ElasticIndexer"
RUN dotnet build "Tradie.ElasticIndexer.csproj" --no-self-contained -r linux-x64 -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tradie.ElasticIndexer.csproj" --no-self-contained -r linux-x64 -c Release -o /app/publish 

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["Tradie.ElasticIndexer::Tradie.ElasticIndexer.Function::FunctionHandler"]
