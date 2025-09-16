
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY services/catalog/src/Catalog.Api/Catalog.Api.csproj services/catalog/src/Catalog.Api/
COPY services/catalog/src/Catalog.Domain/Catalog.Domain.csproj services/catalog/src/Catalog.Domain/
COPY services/catalog/src/Catalog.Infrastructure/Catalog.Infrastructure.csproj services/catalog/src/Catalog.Infrastructure/

RUN dotnet restore services/catalog/src/Catalog.Api/Catalog.Api.csproj

COPY . .

WORKDIR /src/services/catalog/src/Catalog.Api
RUN dotnet publish Catalog.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "Catalog.Api.dll"]
