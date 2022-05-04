FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tradie.Web/Tradie.Web.csproj", "Tradie.Web/"]
RUN dotnet restore "Tradie.Web/Tradie.Web.csproj"
COPY . .
WORKDIR "/src/Tradie.Web"
RUN dotnet build "Tradie.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tradie.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tradie.Web.dll"]
