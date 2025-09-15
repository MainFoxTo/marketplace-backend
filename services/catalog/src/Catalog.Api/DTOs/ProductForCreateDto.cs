using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.DTOs
{
    public class ProductForCreateDto
    {
        [Required(ErrorMessage = "SKU is required")]
        [StringLength(64, ErrorMessage = "SKU cannot exceed 64 characters")]
        public string Sku { get; set; } = default!;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }
    }
}
