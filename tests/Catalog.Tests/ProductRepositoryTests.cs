using Catalog.Domain.Entities;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Tests
{
    public class ProductRepositoryTests
    {
        [Fact]
        public async Task AddAndGetById_Works()
        {
            var options = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase("testdb")
                .Options;

            using var db = new CatalogDbContext(options);
            var repo = new ProductRepository(db);

            var p = new Product { Name = "Test", Sku = "T1" };
            await repo.AddAsync(p);

            var got = await repo.GetByIdAsync(p.Id);
            Assert.NotNull(got);
            Assert.Equal("Test", got!.Name);

        }

    }
}

