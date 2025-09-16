using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.DTOs
{
    public class ProductVersionForCreateDto
    {
        [Required]
        [StringLength(64)]
        public string Sku { get; set; } = default!;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(2000)]
        public string? Metadata { get; set; }
    }
}
