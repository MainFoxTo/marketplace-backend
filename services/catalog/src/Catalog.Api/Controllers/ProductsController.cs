using AutoMapper;
using Catalog.Api.DTOs;
using Catalog.Api.Services;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IMapper _mapper;

        public ProductsController(IProductService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(_mapper.Map<ProductDto[]>(products));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound(new { message = $"Product with id {id} not found" });

            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductForCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var createdProduct = await _service.CreateAsync(product);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                _mapper.Map<ProductDto>(createdProduct));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductForCreateDto dto)
        {
            var existingProduct = await _service.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound(new { message = $"Product with id {id} not found" });

            // Обновляем только разрешенные поля
            existingProduct.Name = dto.Name;
            existingProduct.Sku = dto.Sku;
            existingProduct.Description = dto.Description;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _service.UpdateAsync(existingProduct);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingProduct = await _service.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound(new { message = $"Product with id {id} not found" });

            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id:guid}/versions")]
        public async Task<IActionResult> AddVersion(Guid id, [FromBody] ProductVersionForCreateDto dto)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = $"Product with id {id} not found" });

            var version = _mapper.Map<ProductVersion>(dto);
            version.ProductId = id;

            var createdVersion = await _service.AddVersionAsync(version);

            return CreatedAtAction(
                nameof(GetVersionById),
                new { id = id, versionId = createdVersion.Id },
                _mapper.Map<ProductVersionDto>(createdVersion));
        }

        [HttpGet("{id:guid}/versions")]
        public async Task<IActionResult> GetVersions(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = $"Product with id {id} not found" });

            var versions = await _service.GetVersionsAsync(id);
            return Ok(_mapper.Map<ProductVersionDto[]>(versions));
        }

        [HttpGet("{id:guid}/versions/{versionId:guid}")]
        public async Task<IActionResult> GetVersionById(Guid id, Guid versionId)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = $"Product with id {id} not found" });

            var version = await _service.GetVersionByIdAsync(versionId);
            if (version == null || version.ProductId != id)
                return NotFound(new { message = $"Version {versionId} not found for product {id}" });

            return Ok(_mapper.Map<ProductVersionDto>(version));
        }
    }
}
