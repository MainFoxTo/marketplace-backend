using Catalog.Domain.Entities;

namespace Catalog.Api.Services
{
    public interface IProductService
    {
        Task<Product> CreateAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);

        Task<ProductVersion> AddVersionAsync(ProductVersion v);
        Task<IEnumerable<ProductVersion>> GetVersionsAsync(Guid productId);
    }
}
