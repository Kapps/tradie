#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM 818120129739.dkr.ecr.us-east-1.amazonaws.com/dotnet6:latest AS base
#FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
FROM public.ecr.aws/lambda/dotnet:6 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tradie.Web/Tradie.Web.csproj", "Tradie.Web/"]
RUN dotnet restore "Tradie.Web/Tradie.Web.csproj"
COPY . .
WORKDIR "/src/Tradie.Web"
RUN dotnet build "Tradie.Web.csproj" --no-self-contained -r linux-x64 -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tradie.Web.csproj" --no-self-contained -r linux-x64 -c Release -o /app/publish 

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["Tradie.Web"]
