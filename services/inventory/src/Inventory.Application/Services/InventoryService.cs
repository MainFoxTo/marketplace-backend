using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IStockRepository _repo;
        private readonly InventoryDbContext _dbContext;

        public InventoryService(IStockRepository repo, InventoryDbContext dbContext)
        {
            _repo = repo;
            _dbContext = dbContext;
        }

        public async Task<AvailabilityDto> GetAvailabilityAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var items = await _repo.GetByProductIdAsync(productId, cancellationToken);
            var available = items.Sum(i => Math.Max(0, i.Quantity - i.ReservedQuantity));
            return new AvailabilityDto { Available = available };
        }

        public async Task<ReserveResultDto> ReserveAsync(ReserveRequestDto request, CancellationToken cancellationToken = default)
        {
            // Idempotency: existing reserved by correlation id
            var existingReservations = await _repo.GetReservationsByCorrelationIdAsync(request.CorrelationId, cancellationToken);
            var alreadyReserved = existingReservations.Where(r => r.Status == ReservationStatus.Reserved).ToList();
            if (alreadyReserved.Any())
            {
                return new ReserveResultDto
                {
                    ReservationId = alreadyReserved.First().Id,
                    CorrelationId = request.CorrelationId,
                    ProductId = request.ProductId,
                    Quantity = alreadyReserved.Sum(r => r.Quantity),
                    Status = "AlreadyReserved"
                };
            }

            // Start transaction (DbContext shared instance with repo)
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var stockItem = await _repo.GetByProductIdForUpdateAsync(request.ProductId, cancellationToken);
                if (stockItem == null)
                    throw new Exception($"StockItem for product {request.ProductId} not found.");

                var availableQuantity = stockItem.Quantity - stockItem.ReservedQuantity;
                if (availableQuantity < request.Quantity)
                    throw new InsufficientStockException(request.ProductId, request.Quantity, availableQuantity);

                stockItem.ReservedQuantity += request.Quantity;
                await _repo.UpdateAsync(stockItem);

                var reservation = new Reservation
                {
                    StockItemId = stockItem.Id,
                    CorrelationId = request.CorrelationId,
                    Quantity = request.Quantity,
                    Status = ReservationStatus.Reserved,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30)
                };

                await _repo.AddReservationAsync(reservation);

                await _repo.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new ReserveResultDto
                {
                    ReservationId = reservation.Id,
                    CorrelationId = reservation.CorrelationId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Status = "Reserved",
                    ExpiresAt = reservation.ExpiresAt
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<ReleaseResultDto> ReleaseAsync(ReleaseRequestDto request, CancellationToken cancellationToken = default)
        {
            if (request.ReservationId == null)
                throw new ArgumentException("ReservationId is required in ReleaseRequestDto");

            var reservation = await _repo.GetReservationByIdAsync(request.ReservationId.Value, cancellationToken);
            if (reservation == null)
                throw new Exception($"Reservation {request.ReservationId} not found.");

            if (reservation.Status != ReservationStatus.Reserved)
            {
                return new ReleaseResultDto
                {
                    ReservationId = reservation.Id,
                    ProductId = (await _repo.GetByIdAsync(reservation.StockItemId, cancellationToken))?.ProductId ?? Guid.Empty,
                    Quantity = reservation.Quantity,
                    Status = "AlreadyReleased",
                    ReleasedAt = DateTime.UtcNow
                };
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var stockItem = await _repo.GetByIdAsync(reservation.StockItemId, cancellationToken);
                if (stockItem == null)
                    throw new Exception($"StockItem {reservation.StockItemId} not found.");

                stockItem.ReservedQuantity = Math.Max(0, stockItem.ReservedQuantity - reservation.Quantity);
                reservation.Status = ReservationStatus.Released;

                await _repo.UpdateAsync(stockItem);
                await _repo.UpdateReservationAsync(reservation);

                await _repo.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new ReleaseResultDto
                {
                    ReservationId = reservation.Id,
                    ProductId = stockItem.ProductId,
                    Quantity = reservation.Quantity,
                    Status = "Released",
                    ReleasedAt = DateTime.UtcNow
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
