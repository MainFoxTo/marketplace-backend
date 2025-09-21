using Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Domain.Interfaces
{
    public interface IStockRepository
    {
        Task<IEnumerable<StockItem>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<StockItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<StockItem?> GetByProductIdForUpdateAsync(Guid productId, CancellationToken cancellationToken = default); // new: for UPDATE lock
        Task UpdateAsync(StockItem item);
        Task AddReservationAsync(Reservation reservation);
        Task<IEnumerable<Reservation>> GetReservationsByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default);
        Task<Reservation?> GetReservationByIdAsync(Guid reservationId, CancellationToken cancellationToken = default);
        Task UpdateReservationAsync(Reservation reservation);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
