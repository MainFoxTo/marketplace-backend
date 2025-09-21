using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _svc;
        public InventoryController(IInventoryService svc) => _svc = svc;

        [HttpGet("availability")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailability(
            [FromQuery] Guid productId,
            CancellationToken cancellationToken)
        {
            if (productId == Guid.Empty)
                return BadRequest(new { message = "productId required" });

            var res = await _svc.GetAvailabilityAsync(productId, cancellationToken);
            return Ok(res);
        }

        [HttpPost("reserve")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Reserve(
            [FromBody] ReserveRequestDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _svc.ReserveAsync(dto, cancellationToken);
            // Return created for reserved; if AlreadyReserved - still return 200/202. We'll return 201 for new reservation.
            return CreatedAtAction(nameof(GetAvailability), new { productId = dto.ProductId }, result);
        }

        [HttpPost("release")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Release([FromBody] ReleaseRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await _svc.ReleaseAsync(dto, cancellationToken);
            return Ok(result);
        }
    }
}
