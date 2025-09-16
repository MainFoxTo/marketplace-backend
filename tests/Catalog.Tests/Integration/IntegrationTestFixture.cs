using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace Catalog.Tests.Integration
{
    public class IntegrationTestFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer? _dbContainer;
        private readonly bool _usingProvidedConnection;
        public HttpClient HttpClient { get; private set; } = null!;
        public string ConnectionString { get; private set; } = null!;

        public IntegrationTestFixture()
        {
            // Если в окружении уже есть строка подключения, будем её использовать (CI / docker-compose).
            var envConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            if (!string.IsNullOrEmpty(envConn))
            {
                _usingProvidedConnection = true;
                ConnectionString = envConn;
                _dbContainer = null;
            }
            else
            {
                // Иначе подготовим Testcontainers (локально)
                _usingProvidedConnection = false;
                _dbContainer = new PostgreSqlBuilder()
                    .WithImage("postgres:15")
                    .WithDatabase("test_db")
                    .WithUsername("postgres")
                    .WithPassword("postgres")
                    .Build();
            }
        }

        public async Task InitializeAsync()
        {
            if (!_usingProvidedConnection)
            {
                await _dbContainer!.StartAsync();
                ConnectionString = _dbContainer!.GetConnectionString();
            }

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
            if (!_usingProvidedConnection && _dbContainer != null)
            {
                await _dbContainer.DisposeAsync();
            }

            HttpClient?.Dispose();
        }
    }
}
