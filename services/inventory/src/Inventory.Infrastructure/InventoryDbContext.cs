using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

        public DbSet<StockItem> StockItems { get; set; } = default!;
        public DbSet<Reservation> Reservations { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("inventory");

            modelBuilder.Entity<StockItem>(b =>
            {
                b.ToTable("StockItems");
                b.HasKey(s => s.Id);
                b.Property(s => s.Sku).IsRequired().HasMaxLength(64);
                b.Property(s => s.Quantity).IsRequired();
                b.Property(s => s.ReservedQuantity).IsRequired();
                b.Property(s => s.ProductId).IsRequired();
            });

            modelBuilder.Entity<Reservation>(b =>
            {
                b.ToTable("Reservations");
                b.HasKey(r => r.Id);
                b.Property(r => r.CorrelationId).IsRequired();
                b.Property(r => r.Quantity).IsRequired();
                b.Property(r => r.Status).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
