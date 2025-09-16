using Catalog.Api.Services;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _service = new ProductService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_AddsProduct()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Sku = "TEST123" };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(product);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.Is<Product>(x => x.Sku == "TEST123")), Times.Once);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var expectedProduct = new Product { Id = productId, Name = "Test Product" };
            _mockRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(expectedProduct);

            // Act
            var result = await _service.GetByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.GetByIdAsync(productId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1" },
                new Product { Id = Guid.NewGuid(), Name = "Product 2" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddVersionAsync_AddsProductVersion()
        {
            // Arrange
            var version = new ProductVersion { Sku = "V1", Price = 99.99m };
            _mockRepo.Setup(r => r.AddVersionAsync(It.IsAny<ProductVersion>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.AddVersionAsync(version);

            // Assert
            _mockRepo.Verify(r => r.AddVersionAsync(It.Is<ProductVersion>(x => x.Sku == "V1")), Times.Once);
            Assert.Equal(99.99m, result.Price);
        }
    }
}