FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY services/inventory/src/Inventory.Api/Inventory.Api.csproj services/inventory/src/Inventory.Api/
COPY services/inventory/src/Inventory.Application/Inventory.Application.csproj services/inventory/src/Inventory.Application/
COPY services/inventory/src/Inventory.Domain/Inventory.Domain.csproj services/inventory/src/Inventory.Domain/
COPY services/inventory/src/Inventory.Infrastructure/Inventory.Infrastructure.csproj services/inventory/src/Inventory.Infrastructure/

RUN dotnet restore services/inventory/src/Inventory.Api/Inventory.Api.csproj

COPY . .

WORKDIR /src/services/inventory/src/Inventory.Api
RUN dotnet publish Inventory.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001
ENTRYPOINT ["dotnet", "Inventory.Api.dll"]
