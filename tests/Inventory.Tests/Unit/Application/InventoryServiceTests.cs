using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Inventory.Tests.Unit.Application
{
    public class InventoryServiceTests : IDisposable
    {
        private readonly Mock<IStockRepository> _mockRepository;
        private readonly InventoryDbContext _dbContext;
        private readonly InventoryService _service;
        private readonly SqliteConnection _connection;

        public InventoryServiceTests()
        {
            _mockRepository = new Mock<IStockRepository>();
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseSqlite(_connection)
                .Options;

            _dbContext = new InventoryDbContext(options);
            _dbContext.Database.EnsureCreated();

            _service = new InventoryService(_mockRepository.Object, _dbContext);
        }

        [Fact]
        public async Task GetAvailabilityAsync_ShouldReturnCorrectTotal()
        {
            var productId = Guid.NewGuid();
            var items = new List<StockItem>
            {
                new StockItem { ProductId = productId, Quantity = 100, ReservedQuantity = 20 },
                new StockItem { ProductId = productId, Quantity = 50, ReservedQuantity = 10 }
            };

            _mockRepository.Setup(r => r.GetByProductIdAsync(productId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(items);

            var result = await _service.GetAvailabilityAsync(productId);

            Assert.Equal(120, result.Available);
        }

        [Fact]
        public async Task ReserveAsync_ShouldThrowInsufficientStockException_WhenNotEnoughQuantity()
        {
            var productId = Guid.NewGuid();
            var stockItem = new StockItem { ProductId = productId, Quantity = 10, ReservedQuantity = 10 };

            _mockRepository.Setup(r => r.GetByProductIdForUpdateAsync(productId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(stockItem);

            var request = new ReserveRequestDto { ProductId = productId, Quantity = 5, CorrelationId = Guid.NewGuid() };

            await Assert.ThrowsAsync<InsufficientStockException>(() =>
                _service.ReserveAsync(request, CancellationToken.None));
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _connection.Dispose();
        }
    }
}
