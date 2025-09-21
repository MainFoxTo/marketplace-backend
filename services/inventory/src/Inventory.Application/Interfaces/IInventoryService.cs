using Inventory.Application.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<AvailabilityDto> GetAvailabilityAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<ReserveResultDto> ReserveAsync(ReserveRequestDto request, CancellationToken cancellationToken = default);
        Task<ReleaseResultDto> ReleaseAsync(ReleaseRequestDto request, CancellationToken cancellationToken = default);
    }
}