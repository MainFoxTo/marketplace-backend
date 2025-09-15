using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo) => _repo = repo;

        public async Task<Product> CreateAsync(Product product)
        {
            await _repo.AddAsync(product);
            return product;
        }

        public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);

        public async Task<IEnumerable<Product>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Product?> GetByIdAsync(Guid id) => await _repo.GetByIdAsync(id);

        public async Task UpdateAsync(Product product) => await _repo.UpdateAsync(product);

        public async Task<ProductVersion> AddVersionAsync(ProductVersion version)
        {
            await _repo.AddVersionAsync(version);
            return version;
        }

        public async Task<IEnumerable<ProductVersion>> GetVersionsAsync(Guid productId) =>
            await _repo.GetVersionsAsync(productId);
    }
}
