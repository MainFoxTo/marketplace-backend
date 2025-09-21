using Inventory.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.Tests.Integration
{
    [Collection("IntegrationTests")]
    public class InventoryControllerIntegrationTests
    {
        private readonly HttpClient _client;

        public InventoryControllerIntegrationTests(IntegrationTestFixture fixture)
        {
            _client = fixture.HttpClient;
        }

        [Fact]
        public async Task HealthEndpoint_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/health");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAvailability_WithInvalidProductId_ShouldReturnBadRequest()
        {
            var response = await _client.GetAsync("/api/inventory/availability?productId=00000000-0000-0000-0000-000000000000");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Reserve_WithInvalidRequest_ShouldReturnValidationError()
        {
            var dto = new ReserveRequestDto
            {
                ProductId = Guid.Empty,
                Quantity = 0,
                CorrelationId = Guid.Empty
            };

            var response = await _client.PostAsJsonAsync("/api/inventory/reserve", dto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
