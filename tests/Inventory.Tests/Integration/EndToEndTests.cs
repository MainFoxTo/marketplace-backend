using Inventory.Application.DTOs;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.Tests.Integration
{
    [Collection("IntegrationTests")]
    public class EndToEndTests
    {
        private readonly HttpClient _client;

        public EndToEndTests(IntegrationTestFixture fixture)
        {
            _client = fixture.HttpClient;
        }

        [Fact]
        public async Task ReserveFlow_ShouldWorkCorrectly()
        {
            var productId = IntegrationTestFixture.SeededProductId;

            // Проверка доступности перед резервом
            var preAvailability = await _client.GetAsync($"/api/inventory/availability?productId={productId}");
            var preBody = await preAvailability.Content.ReadAsStringAsync();
            Console.WriteLine($"Pre-availability response: {preBody}");
            preAvailability.EnsureSuccessStatusCode();

            var reserveRequest = new ReserveRequestDto
            {
                ProductId = productId,
                Quantity = 5,
                CorrelationId = System.Guid.NewGuid()
            };

            var reserveResponse = await _client.PostAsJsonAsync("/api/inventory/reserve", reserveRequest);

           
            Assert.True(reserveResponse.IsSuccessStatusCode, await reserveResponse.Content.ReadAsStringAsync());

            // Проверка доступности после резерва
            var availabilityResponse = await _client.GetAsync($"/api/inventory/availability?productId={productId}");
            availabilityResponse.EnsureSuccessStatusCode();

            var availability = await availabilityResponse.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(availability);
        }
    }
}
