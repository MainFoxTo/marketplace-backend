// services/inventory/src/Inventory.Infrastructure/Repositories/StockRepository.cs
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly InventoryDbContext _db;
        public StockRepository(InventoryDbContext db) => _db = db;

        // Get by product id (read-only)
        public async Task<IEnumerable<StockItem>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default) =>
            await _db.StockItems
                     .AsNoTracking()
                     .Where(s => s.ProductId == productId)
                     .ToListAsync(cancellationToken);

        // FOR UPDATE: lock a single stock item row for the product (we pick the first matching row)
        public async Task<StockItem?> GetByProductIdForUpdateAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            // Parameterized interpolated SQL to avoid injection
            return await _db.StockItems
                .FromSqlInterpolated($"SELECT * FROM inventory.\"StockItems\" WHERE \"ProductId\" = {productId} FOR UPDATE")
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<StockItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await _db.StockItems.FindAsync(new object[] { id }, cancellationToken);

        public async Task<IEnumerable<Reservation>> GetReservationsByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default) =>
            await _db.Reservations
                     .AsNoTracking()
                     .Where(r => r.CorrelationId == correlationId)
                     .ToListAsync(cancellationToken);

        public async Task<Reservation?> GetReservationByIdAsync(Guid reservationId, CancellationToken cancellationToken = default) =>
            await _db.Reservations
                     .FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);

        public Task AddReservationAsync(Reservation reservation)
        {
            _db.Reservations.Add(reservation);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(StockItem item)
        {
            _db.StockItems.Update(item);
            return Task.CompletedTask;
        }

        public Task UpdateReservationAsync(Reservation reservation)
        {
            _db.Reservations.Update(reservation);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _db.SaveChangesAsync(cancellationToken);
    }
}