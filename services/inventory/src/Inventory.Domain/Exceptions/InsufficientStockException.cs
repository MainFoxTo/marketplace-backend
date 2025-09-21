// services/inventory/src/Inventory.Domain/Exceptions/InsufficientStockException.cs
using System;

namespace Inventory.Domain.Exceptions
{
    // Видимость — public, чтобы использовать из других проектов (Application)
    public class InsufficientStockException : Exception
    {
        public Guid ProductId { get; }
        public int RequestedQuantity { get; }
        public int AvailableQuantity { get; }

        public InsufficientStockException(Guid productId, int requestedQuantity, int availableQuantity)
            : base($"Insufficient stock for product {productId}. Requested: {requestedQuantity}, Available: {availableQuantity}")
        {
            ProductId = productId;
            RequestedQuantity = requestedQuantity;
            AvailableQuantity = availableQuantity;
        }
    }
}
