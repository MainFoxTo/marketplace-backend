using Inventory.Domain.Entities;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Inventory.Tests.Unit.Infrastructure
{
    public class StockRepositoryTests : IDisposable
    {
        private readonly InventoryDbContext _context;
        private readonly StockRepository _repository;
        private readonly SqliteConnection _connection;

        public StockRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new InventoryDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new StockRepository(_context);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingItem_ReturnsStockItem()
        {
            var stockItem = new StockItem { Sku = "TEST-1", Quantity = 100, ProductId = Guid.NewGuid() };
            _context.StockItems.Add(stockItem);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(stockItem.Id);

            Assert.NotNull(result);
            Assert.Equal(stockItem.Id, result.Id);
        }

        [Fact]
        public async Task GetByProductIdAsync_ReturnsCorrectItems()
        {
            var productId = Guid.NewGuid();
            _context.StockItems.AddRange(
                new StockItem { ProductId = productId, Quantity = 10, Sku = "A" },
                new StockItem { ProductId = productId, Quantity = 20, Sku = "B" }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetByProductIdAsync(productId);
            Assert.Equal(2, result.Count());
            Assert.All(result, s => Assert.Equal(productId, s.ProductId));
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
