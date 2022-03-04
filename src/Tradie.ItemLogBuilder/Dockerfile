#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# FROM 818120129739.dkr.ecr.us-east-1.amazonaws.com/dotnet6:latest AS base
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tradie.ItemLogBuilder/Tradie.ItemLogBuilder.csproj", "Tradie.ItemLogBuilder/"]
RUN dotnet restore "Tradie.ItemLogBuilder/Tradie.ItemLogBuilder.csproj"
COPY . .
WORKDIR "/src/Tradie.ItemLogBuilder"
RUN dotnet build "Tradie.ItemLogBuilder.csproj" -r linux-x64 -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tradie.ItemLogBuilder.csproj" -r linux-x64 -c Release -o /app/publish

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["Tradie.ItemLogBuilder::Tradie.ItemLogBuilder.Function::FunctionHandler"]
