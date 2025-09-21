using Inventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StockItemId { get; set; }
        public Guid CorrelationId { get; set; } // идемпотентность
        public int Quantity { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Reserved;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
    }
}
