using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly CatalogDbContext _db;
        public ProductRepository(CatalogDbContext db) => _db = db;

        public async Task AddAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p != null)
            {
                _db.Products.Remove(p);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _db.Products
                     .AsNoTracking()
                     .Include(p => p.Versions)
                     .ToListAsync();

        public async Task<Product?> GetByIdAsync(Guid id) =>
            await _db.Products
                     .AsNoTracking()
                     .Include(p => p.Versions)
                     .FirstOrDefaultAsync(p => p.Id == id);

        public async Task UpdateAsync(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task AddVersionAsync(ProductVersion version)
        {
            _db.ProductVersions.Add(version);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductVersion>> GetVersionsAsync(Guid productId) =>
            await _db.ProductVersions
                     .AsNoTracking()
                     .Where(v => v.ProductId == productId)
                     .ToListAsync();

        public async Task<ProductVersion?> GetVersionByIdAsync(Guid versionId) =>
            await _db.ProductVersions
                     .AsNoTracking()
                     .FirstOrDefaultAsync(v => v.Id == versionId);
    }
}
