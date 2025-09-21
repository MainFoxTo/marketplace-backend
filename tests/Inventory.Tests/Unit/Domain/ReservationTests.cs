using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using System;
using Xunit;

namespace Inventory.Tests.Unit.Domain
{
    public class ReservationTests
    {
        [Fact]
        public void DefaultStatus_ShouldBeReserved()
        {
            var r = new Reservation();
            Assert.Equal(ReservationStatus.Reserved, r.Status);
        }

        [Fact]
        public void CanChangeStatus()
        {
            var r = new Reservation { Status = ReservationStatus.Reserved };
            r.Status = ReservationStatus.Released;
            Assert.Equal(ReservationStatus.Released, r.Status);
        }

        [Fact]
        public void CorrelationId_ShouldSupportIdempotency()
        {
            var id = Guid.NewGuid();
            var r1 = new Reservation { CorrelationId = id };
            var r2 = new Reservation { CorrelationId = id };
            Assert.Equal(r1.CorrelationId, r2.CorrelationId);
        }
    }
}
