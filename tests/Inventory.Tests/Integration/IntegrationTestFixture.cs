using Inventory.Api;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace Inventory.Tests.Integration
{
    [CollectionDefinition("IntegrationTests")]
    public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture> { }

    public class IntegrationTestFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        private WebApplicationFactory<Program>? _factory;

        public HttpClient HttpClient { get; private set; } = null!;
        public string ConnectionString { get; private set; } = null!;

        public static readonly Guid SeededProductId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public IntegrationTestFixture()
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:15.1")
                .WithDatabase("test_db")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();
        }

        public async Task InitializeAsync()
        {
            // 1️⃣ Запуск Postgres контейнера
            await _dbContainer.StartAsync();
            ConnectionString = _dbContainer.GetConnectionString();

            // 2️⃣ Настройка WebApplicationFactory
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");

                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        var dict = new Dictionary<string, string?>
                        {
                            ["ConnectionStrings:DefaultConnection"] = ConnectionString
                        };
                        config.AddInMemoryCollection(dict);
                    });
                });

            // 3️⃣ Накат миграций
            var dbOptions = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            using (var db = new InventoryDbContext(dbOptions))
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.MigrateAsync();
            }

            // 4️⃣ Seed StockItem через репозиторий
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
                var repo = new Inventory.Infrastructure.Repositories.StockRepository(db);

                var existing = await repo.GetByProductIdAsync(SeededProductId);
                if (!existing.Any())
                {
                    db.StockItems.Add(new StockItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = SeededProductId,
                        Sku = "SEED-SKU-001",
                        Quantity = 100,
                        ReservedQuantity = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    await db.SaveChangesAsync();
                }
            }

            // 5️⃣ Создаём HttpClient
            HttpClient = _factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            HttpClient?.Dispose();
            _factory?.Dispose();
            await _dbContainer.DisposeAsync();
        }
    }
}
