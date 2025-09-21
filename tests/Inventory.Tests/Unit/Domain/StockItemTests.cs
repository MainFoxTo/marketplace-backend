using Inventory.Domain.Entities;
using System;
using Xunit;

namespace Inventory.Tests.Unit.Domain
{
    public class StockItemTests
    {
        [Fact]
        public void AvailableQuantity_ShouldBeQuantityMinusReserved()
        {
            var s = new StockItem { Quantity = 100, ReservedQuantity = 30 };
            Assert.Equal(70, s.Quantity - s.ReservedQuantity);
        }

        [Fact]
        public void ShouldSetDefaultDates()
        {
            var s = new StockItem();
            Assert.NotEqual(default, s.CreatedAt);
            Assert.NotEqual(default, s.UpdatedAt);
        }
    }
}
