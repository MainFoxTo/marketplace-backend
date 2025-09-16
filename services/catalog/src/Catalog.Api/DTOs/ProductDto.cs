using System;
using System.Collections.Generic;

namespace Catalog.Api.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Список версий продукта
        public List<ProductVersionDto> Versions { get; set; } = new();
    }
}
