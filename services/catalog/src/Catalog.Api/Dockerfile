
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["services/catalog/src/Catalog.Api/Catalog.Api.csproj", "Catalog.Api/"]
COPY ["services/catalog/src/Catalog.Domain/Catalog.Domain.csproj", "Catalog.Domain/"]
COPY ["services/catalog/src/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "Catalog.Infrastructure/"]
RUN dotnet restore "Catalog.Api/Catalog.Api.csproj"

COPY . .
WORKDIR "/src/Catalog.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "Catalog.Api.dll"]