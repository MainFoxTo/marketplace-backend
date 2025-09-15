using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.DTOs
{
    public class ProductVersionDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Sku { get; set; } = default!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
