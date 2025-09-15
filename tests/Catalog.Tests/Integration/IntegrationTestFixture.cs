using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace Catalog.Tests.Integration
{
    public class IntegrationTestFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;

        public IntegrationTestFixture()
        {
            // Запускаем контейнер с PostgreSQL
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:15")
                .WithDatabase("test_db")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();
        }

        public HttpClient HttpClient { get; private set; } = null!;
        public string ConnectionString { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            ConnectionString = _dbContainer.GetConnectionString();

            // Создаем фабрику приложения с подменой строки подключения
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        // Переопределяем строку подключения для тестов
                        config.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["ConnectionStrings:DefaultConnection"] = ConnectionString
                        }!);
                    });
                });

            HttpClient = application.CreateClient();
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
            HttpClient?.Dispose();
        }
    }
}
