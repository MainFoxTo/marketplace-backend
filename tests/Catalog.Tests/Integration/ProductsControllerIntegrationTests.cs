using Catalog.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Tests.Integration
{
    // Указываем, что эти тесты принадлежат к коллекции "IntegrationTests"
    [Collection("IntegrationTests")]
    public class ProductsControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly HttpClient _httpClient;

        public ProductsControllerIntegrationTests(IntegrationTestFixture fixture)
        {
            _httpClient = fixture.HttpClient;
        }

        [Fact]
        public async Task FullProductLifecycle_ShouldWork()
        {
            // Arrange
            var newProduct = new ProductForCreateDto
            {
                Sku = "INTEGRATION_TEST_001",
                Name = "Integration Test Product",
                Description = "Product created during integration testing"
            };

            // Act 1: Create Product
            var createResponse = await _httpClient.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

            // Assert 1: Product was created
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            Assert.NotNull(createdProduct);
            Assert.NotNull(createdProduct.Id);
            Assert.Equal(newProduct.Sku, createdProduct.Sku);
            Assert.Equal(newProduct.Name, createdProduct.Name);

            // Act 2: Get Created Product
            var getResponse = await _httpClient.GetAsync($"/api/products/{createdProduct.Id}");
            var fetchedProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>();

            // Assert 2: Product can be retrieved
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(fetchedProduct);
            Assert.Equal(createdProduct.Id, fetchedProduct.Id);
            Assert.Equal(createdProduct.Sku, fetchedProduct.Sku);

            // Act 3: Update Product
            var updateDto = new ProductForCreateDto
            {
                Sku = "UPDATED_TEST_001",
                Name = "Updated Integration Test Product",
                Description = "Updated description"
            };

            var updateResponse = await _httpClient.PutAsJsonAsync($"/api/products/{createdProduct.Id}", updateDto);

            // Assert 3: Update was successful
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Act 4: Get Updated Product
            var getUpdatedResponse = await _httpClient.GetAsync($"/api/products/{createdProduct.Id}");
            var updatedProduct = await getUpdatedResponse.Content.ReadFromJsonAsync<ProductDto>();

            // Assert 4: Product was updated
            Assert.Equal(HttpStatusCode.OK, getUpdatedResponse.StatusCode);
            Assert.NotNull(updatedProduct);
            Assert.Equal(updateDto.Name, updatedProduct.Name);
            Assert.Equal(updateDto.Sku, updatedProduct.Sku);

            // Act 5: Delete Product
            var deleteResponse = await _httpClient.DeleteAsync($"/api/products/{createdProduct.Id}");

            // Assert 5: Delete was successful
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Act 6: Try to Get Deleted Product
            var getDeletedResponse = await _httpClient.GetAsync($"/api/products/{createdProduct.Id}");

            // Assert 6: Product not found
            Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
        }
    }
}
